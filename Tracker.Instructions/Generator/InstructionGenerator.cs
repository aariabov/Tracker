using System.Diagnostics;
using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions.Generator;

public class InstructionGenerator
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

    private InstructionsMemoryStorage Instructions = new();

    private readonly Random _random;

    public InstructionGenerator()
    {
        _random = new Random();
    }

    public void Benchmark(int total, User[] bosses)
    {
        Console.WriteLine($"Cores count {Environment.ProcessorCount}");

        var sw = Stopwatch.StartNew();
        GenerateForLoop(total, bosses);
        Console.WriteLine("ForLoop: {0:f2} s", sw.Elapsed.TotalSeconds);

        Instructions = new();
        sw = Stopwatch.StartNew();
        GenerateParallelFor(total, bosses);
        Console.WriteLine("Parallel.For: {0:f2} s", sw.Elapsed.TotalSeconds);

        Instructions = new();
        sw = Stopwatch.StartNew();
        GenerateParallelForDegreeOfParallelism(total, bosses);
        Console.WriteLine("ParallelForDegreeOfParallelism: {0:f2} s", sw.Elapsed.TotalSeconds);

        Instructions = new();
        sw = Stopwatch.StartNew();
        GenerateCustomParallel(total, bosses);
        Console.WriteLine("CustomParallel: {0:f2} s", sw.Elapsed.TotalSeconds);
    }

    public Instruction[] GenerateForLoop(int total, User[] bosses)
    {
        for (var i = 1; i <= total; i++)
        {
            GenerateInstruction(bosses, i);
        }

        return Instructions.Values.ToArray();
    }

    public Instruction[] GenerateParallelFor(int total, User[] bosses)
    {
        Parallel.For(1, total + 1, i => { GenerateInstruction(bosses, i); });
        return Instructions.Values.ToArray();
    }

    public void GenerateParallelForDegreeOfParallelism(int total, User[] bosses)
    {
        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        Parallel.For(1, total + 1, options, i => { GenerateInstruction(bosses, i); });
    }

    public Instruction[] GenerateCustomParallel(int total, User[] bosses)
    {
        var degreeOfParallelism = Environment.ProcessorCount;
        var tasks = new Task[degreeOfParallelism];

        for (var taskNumber = 0; taskNumber < degreeOfParallelism; taskNumber++)
        {
            // делаем копию, чтобы в лямбде корректно работало
            var taskNumberCopy = taskNumber;
            var from = total * taskNumberCopy / degreeOfParallelism;
            var to = total * (taskNumberCopy + 1) / degreeOfParallelism;

            tasks[taskNumber] = Task.Factory.StartNew(() =>
                {
                    for (var i = from; i < to; i++)
                    {
                        GenerateInstruction(bosses, i);
                    }
                });
        }

        Task.WaitAll(tasks);
        return Instructions.Values.ToArray();
    }

    private void GenerateInstruction(User[] bosses, int i)
    {
        // рандомно выбираем создателя, исполнителя, день создания(pastDay) и deadline поручения
        var bossIdx = _random.Next(0, bosses.Length);
        var creator = bosses[bossIdx];

        var executorIdx = _random.Next(0, creator.Children.Count);
        var executor = creator.Children.ToArray()[executorIdx];

        var pastDay = DateTime.UtcNow.Date.AddDays(_random.Next(MinPastDaysFromToday, 0));
        var deadline = pastDay.AddDays(_random.Next(0, MaxDeadlineDaysFromToday));

        var newInstruction = new Instruction
        {
            Name = $"Поручение {i}",
            CreatorId = creator.Id,
            ExecutorId = executor.Id,
            Deadline = deadline,
        };
        var newId = Instructions.Add(newInstruction);
        newInstruction.TreePath = newId.ToString();

        // тк не все поручения делегируются, рандомно решаем, будем ли делегировать
        if (_random.NextDouble() > DelegationThreshold)
        {
            Delegate(executor, newId, i.ToString(), pastDay, deadline);
        }
    }

    private DateTime? Delegate(User creator, int parentInstructionId, string prefix, DateTime pastDay, DateTime parentDeadline)
    {
        if (creator.Children is null || !creator.Children.Any())
        {
            // тк дошли до листового юзера, можно исполнить поручение, но не обязательно
            return MaybeExecuteInstruction(creator.Id, parentInstructionId, pastDay, parentDeadline);
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

            var parentInstruction = Instructions[parentInstructionId];
            var newInstruction = new Instruction
            {
                Name = $"Поручение {newPrefix}",
                CreatorId = creator.Id,
                ExecutorId = executor.Id,
                Deadline = deadline,
                ParentId = parentInstructionId,
            };
            var newId = Instructions.Add(newInstruction);
            newInstruction.TreePath = $"{parentInstruction.TreePath}{TreePathsService.TreePathDelimiter}{newId}";

            // рекурсивно делегируем потомкам
            var execDate = Delegate(executor, newId, newPrefix, pastDay, deadline);
            childrenExecDates.Add(execDate);

            i++;
        }

        // если не было делегирования - можем исполнить
        if (!childrenExecDates.Any())
        {
            return MaybeExecuteInstruction(creator.Id, parentInstructionId, pastDay, parentDeadline);
        }

        // если есть хотя бы одно неисполненное дочернее поручение - исполнять нельзя
        if (childrenExecDates.Any(d => d is null))
        {
            return null;
        }

        // если все потомки исполнили - можем исполнить, но только с датой >= макс дата исполнения всех потомков
        return MaybeExecuteInstruction(creator.Id, parentInstructionId, childrenExecDates.Max().Value, parentDeadline);
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

    private DateTime? MaybeExecuteInstruction(string executorId, int instructionId, DateTime pastDay,
        DateTime deadline)
    {
        if (_random.NextDouble() < ExecutionThreshold)
        {
            return null;
        }

        return ExecuteInstruction(executorId, instructionId, pastDay, deadline);
    }

    private DateTime ExecuteInstruction(string executorId, int instructionId, DateTime minPossibleDate, DateTime deadline)
    {
        var execDate = GenerateExecDate(minPossibleDate, deadline);
        var instruction = Instructions[instructionId];
        instruction.ExecDate = execDate.Date;

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

    private class InstructionsMemoryStorage : Dictionary<int, Instruction>
    {
        private readonly object _locker = new();

        public int Add(Instruction newInstruction)
        {
            lock (_locker)
            {
                var nextId = Count + 1;
                newInstruction.Id = nextId;
                base.Add(nextId, newInstruction);
                return nextId;
            }
        }
    }
}

