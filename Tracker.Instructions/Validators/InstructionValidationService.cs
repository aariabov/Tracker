using Riabov.Tracker.Common;
using Tracker.Instructions.RequestModels;
using Tracker.Instructions.Validators.FluentValidators;
using User = Tracker.Instructions.Db.Models.User;

namespace Tracker.Instructions.Validators;

public class InstructionValidationService
{
    private readonly InstructionsRepository _instructionsRepository;
    private readonly InstructionStatusService _statusService;
    private readonly UserRepository _userRepository;

    public InstructionValidationService(InstructionsRepository instructionsRepository
        , InstructionStatusService statusService, UserRepository userRepository)
    {
        _instructionsRepository = instructionsRepository;
        _statusService = statusService;
        _userRepository = userRepository;
    }

    public async Task<Result> ValidateInstructionAsync(InstructionRm instructionRm, User creator, DateTime today)
    {
        var validator = new InstructionValidator(_instructionsRepository, _userRepository, _statusService, creator, today);
        var validationResult = await validator.ValidateAsync(instructionRm);
        if (validationResult.IsValid)
        {
            return Result.Ok();
        }

        return Result.Errors<string>(validationResult.Errors.Format());
    }

    public async Task<Result> ValidateExecDateAsync(ExecDateRm execDateRm, string executorId, DateTime today)
    {
        var validator = new ExecDateValidator(_instructionsRepository, _statusService, executorId, today);
        var validationResult = await validator.ValidateAsync(execDateRm);
        if (validationResult.IsValid)
        {
            return Result.Ok();
        }

        return Result.Errors<string>(validationResult.Errors.Format());
    }
}
