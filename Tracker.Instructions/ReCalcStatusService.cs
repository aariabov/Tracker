namespace Tracker.Instructions;

public class ReCalcStatusService
{
    private readonly InstructionsRepository _instructionsRepository;
    private readonly InstructionStatusService _statusService;

    public ReCalcStatusService(InstructionsRepository instructionsRepository, InstructionStatusService statusService)
    {
        _instructionsRepository = instructionsRepository;
        _statusService = statusService;
    }

    /// <summary>
    /// Benchmark: Для каждого поручения считаем статус. Вызываем SaveChanges на каждой итерации
    /// </summary>
    public async Task RecalculateStatusesSavePerIteration()
    {
        await _instructionsRepository.UpdateAllStatusesToNullAsync();

        var instructionIds = await _instructionsRepository.GetInstructionIdsAsync();
        foreach (var instructionId in instructionIds)
        {
            var instruction = await _instructionsRepository.GetInstructionTreeAsync(instructionId);
            _statusService.ReCalcStatus(instruction);
            _instructionsRepository.UpdateInstruction(instruction);
            await _instructionsRepository.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Benchmark: Для каждого поручения считаем статус. Вызываем SaveChanges один раз в самом конце
    /// </summary>
    public async Task RecalculateStatusesSingleSave()
    {
        await _instructionsRepository.UpdateAllStatusesToNullAsync();

        var instructionIds = await _instructionsRepository.GetInstructionIdsAsync();
        foreach (var instructionId in instructionIds)
        {
            var instruction = await _instructionsRepository.GetInstructionTreeAsync(instructionId);
            _statusService.ReCalcStatus(instruction);
            _instructionsRepository.UpdateInstruction(instruction);
        }
        await _instructionsRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Benchmark: Считаем статус только для root поручений. Для дочерних считается автоматом.
    /// Вызываем SaveChanges один раз в самом конце
    /// </summary>
    public async Task RecalculateStatusesForRoot()
    {
        await _instructionsRepository.UpdateAllStatusesToNullAsync();

        var instructionIds = await _instructionsRepository.GetRootInstructionIdsAsync();
        foreach (var instructionId in instructionIds)
        {
            var instruction = await _instructionsRepository.GetInstructionTreeAsync(instructionId);
            _statusService.ReCalcStatus(instruction);
            _instructionsRepository.UpdateInstruction(instruction);
        }
        await _instructionsRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Benchmark: Считаем статус только для root поручений, которые в работе, тк для завершенных нет смысла пересчитывать.
    /// Для дочерних считается автоматом.
    /// Вызываем SaveChanges один раз в самом конце
    /// </summary>
    public async Task RecalculateStatusesForRootInWork()
    {
        var instructionIds = await _instructionsRepository.GetInWorkRootInstructionIdsAsync();
        foreach (var instructionId in instructionIds)
        {
            var instruction = await _instructionsRepository.GetInstructionTreeAsync(instructionId);
            _statusService.ReCalcStatus(instruction);
            _instructionsRepository.UpdateInstruction(instruction);
            await _instructionsRepository.SaveChangesAsync();
        }
        await _instructionsRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Пересчитываем статусы только у поручений, которые в работе и у которых наступил deadline
    /// </summary>
    public async Task RecalculateStatusesForRootInWorkAndDeadlineLessNow()
    {
        await Task.Delay(TimeSpan.FromSeconds(20));
        var instructionIds = await _instructionsRepository.GetReCalcStatusRootInstructionIds();
        foreach (var instructionId in instructionIds)
        {
            var instruction = await _instructionsRepository.GetInstructionTreeAsync(instructionId);
            _statusService.ReCalcStatus(instruction);
            _instructionsRepository.UpdateInstruction(instruction);
        }
        await _instructionsRepository.SaveChangesAsync();
    }
}
