using System.Diagnostics;
using Riabov.Tracker.Common.Progress;
using Tracker.Db.Models;
using Tracker.Instructions.RequestModels;
using Tracker.Users;

namespace Tracker.Instructions;

public class InstructionSlowGeneratorService
{
    private const int MinPastDaysFromToday = -365;
    private const int MaxDeadlineDaysFromToday = 10;
    private const int MaxDaysFromParentDeadline = 4;
    private const int MaxOverdueDays = 4;
    private const double OverdueThreshold = 0.3;
    private const double DelegationThreshold = 0.1;
    private const double DelegationToChildThreshold = 0.5;
    private const double ChildDeadlineThreshold = 0.1;
    private const double ExecutionThreshold = 0.1;

    private readonly UsersService _usersService;
    private readonly IInstructionsService _instructionsService;
    private readonly Progress _progress;
    private readonly Random _random;

    public InstructionSlowGeneratorService(UsersService usersService, IInstructionsService instructionsService, Progress progress)
    {
        _usersService = usersService;
        _instructionsService = instructionsService;
        _progress = progress;
        _random = new Random();
    }

    [Obsolete("Медленный генератор: работает синхронно, каждое поручение сначала генерируется потом инсертится, выполняется валидация, прогресс")]
    public async Task RunJob(ClientSocketRm socket, GenerationRm model, int taskId)
    {
        var watch = Stopwatch.StartNew();

        var allUsers = await _usersService.GetUsersTreeAsync();
        var bosses = allUsers.Where(u => u.Children != null && u.Children.Any()).ToArray();

        for (var i = 1; i < model.Total; i++)
        {
            // рандомно выбираем создателя, исполнителя, день создания(pastDay) и deadline поручения
            var bossIdx = _random.Next(0, bosses.Length);
            var creator = bosses[bossIdx];

            var executorIdx = _random.Next(0, creator.Children.Count);
            var executor = creator.Children.ToArray()[executorIdx];

            var pastDay = DateTime.UtcNow.Date.AddDays(_random.Next(MinPastDaysFromToday, 0));
            var deadline = pastDay.AddDays(_random.Next(0, MaxDeadlineDaysFromToday));

            var instruction = new InstructionRm($"Поручение {i}", executor.Id, deadline, parentId: null);
            var res = await _instructionsService.CreateInstructionAsync(instruction, creator, pastDay);
            if (!res.IsSuccess)
            {
                throw new Exception(string.Join(", ", res.ValidationErrors));
            }

            // тк не все поручения делегируются, рандомно решаем, будем ли делегировать
            if (_random.NextDouble() > DelegationThreshold)
            {
                await Delegate(executor, res.Value, i.ToString(), pastDay, deadline);
            }

            _progress.NotifyClient(i, model.Total, socket, frequency: 1, taskId);
        }

        watch.Stop();
        Console.WriteLine($"Generation time: {watch.Elapsed:mm\\:ss\\.ff}");
    }

    private async Task<DateTime?> Delegate(User creator, int parentInstructionId, string prefix, DateTime pastDay, DateTime parentDeadline)
    {
        if (creator.Children is null || !creator.Children.Any())
        {
            // тк дошли до листового юзера, можно исполнить поручение, но не обязательно
            return await MaybeExecuteInstruction(creator.Id, parentInstructionId, pastDay, parentDeadline);
        }

        var i = 1;
        var childrenExecDates = new List<DateTime?>();
        foreach (var executor in creator.Children)
        {
            // можем делегировать не на всех детей, рандомно решаем
            if (_random.NextDouble() < DelegationToChildThreshold)
            {
                continue;
            }

            var deadline = GenerateDeadline(pastDay, parentDeadline);
            var newPrefix = $"{prefix}.{i}";
            var instruction = new InstructionRm($"Поручение {newPrefix}", executor.Id, deadline, parentInstructionId);

            var res = await _instructionsService.CreateInstructionAsync(instruction, creator, pastDay);
            if (!res.IsSuccess)
            {
                throw new Exception(string.Join(", ", res.ValidationErrors));
            }

            // рекурсивно делегируем потомкам
            var execDate = await Delegate(executor, res.Value, newPrefix, pastDay, deadline);
            childrenExecDates.Add(execDate);

            i++;
        }

        // если не было делегирования - можем исполнить
        if (!childrenExecDates.Any())
        {
            return await MaybeExecuteInstruction(creator.Id, parentInstructionId, pastDay, parentDeadline);
        }

        // если есть хотя бы одно неисполненное дочернее поручение - исполнять нельзя
        if (childrenExecDates.Any(d => d is null))
        {
            return null;
        }

        // если все потомки исполнили - можем исполнить, но только с датой >= макс дата исполнения всех потомков
        return await MaybeExecuteInstruction(creator.Id, parentInstructionId, childrenExecDates.Max().Value, parentDeadline);
    }

    private DateTime GenerateDeadline(DateTime pastDay, DateTime parentDeadline)
    {
        // в некоторых случаях дедлайн дочернего поручения мб больше родительского
        if (_random.NextDouble() < ChildDeadlineThreshold)
        {
            return parentDeadline.AddDays(_random.Next(1, MaxDaysFromParentDeadline));
        }

        // в большинстве случаев дедлайн дочернего поручения меньше родительского
        var diffDays = (parentDeadline - pastDay).Days;
        return pastDay.AddDays(_random.Next(0, diffDays));
    }

    private async Task<DateTime?> MaybeExecuteInstruction(string executorId, int instructionId, DateTime pastDay,
        DateTime deadline)
    {
        if (_random.NextDouble() < ExecutionThreshold)
        {
            return null;
        }

        return await ExecuteInstruction(executorId, instructionId, pastDay, deadline);
    }

    private async Task<DateTime> ExecuteInstruction(string executorId, int instructionId, DateTime minPossibleDate, DateTime deadline)
    {
        var execDate = GenerateExecDate(minPossibleDate, deadline);
        var execDateRm = new ExecDateRm { ExecDate = execDate, InstructionId = instructionId };
        var res = await _instructionsService.SetExecDateAsync(execDateRm, executorId, execDate);
        if (!res.IsSuccess)
        {
            throw new Exception(string.Join(", ", res.ValidationErrors));
        }

        return execDate.Date;
    }

    private DateTime GenerateExecDate(DateTime minPossibleDate, DateTime deadline)
    {
        // когда кто-то из потомков просрочил, и дата исполнения больше родительского дедлайна
        if (minPossibleDate > deadline)
        {
            return minPossibleDate.AddDays(_random.Next(0, MaxOverdueDays));
        }

        // просрачиваем
        if (_random.NextDouble() < OverdueThreshold)
        {
            return deadline.AddDays(_random.Next(1, MaxOverdueDays));
        }

        // исполняем вовремя
        var diffDays = (deadline - minPossibleDate).Days;
        return minPossibleDate.AddDays(_random.Next(0, diffDays));
    }
}
