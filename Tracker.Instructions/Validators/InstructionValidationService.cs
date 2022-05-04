using Tracker.Common;
using Tracker.Instructions.RequestModels;
using Tracker.Instructions.Validators.FluentValidators;
using Tracker.Users;

namespace Tracker.Instructions.Validators;

public class InstructionValidationService
{
    private readonly IInstructionsRepository _instructionsRepository;
    private readonly UsersService _usersService;
    private readonly IInstructionStatusService _statusService;

    public InstructionValidationService(IInstructionsRepository instructionsRepository
        , UsersService usersService
        , IInstructionStatusService statusService)
    {
        _instructionsRepository = instructionsRepository;
        _usersService = usersService;
        _statusService = statusService;
    }

    public async Task<Result> ValidateInstructionAsync(InstructionRm instructionRm)
    {
        var validator = new InstructionValidator(_instructionsRepository, _usersService, _statusService);
        var validationResult = await validator.ValidateAsync(instructionRm);
        if (validationResult.IsValid)
            return Result.Ok();
            
        return Result.Errors<string>(validationResult.Errors.Format());
    }
    
    public async Task<Result> ValidateExecDateAsync(ExecDateRm execDateRm)
    {
        var validator = new ExecDateValidator(_instructionsRepository, _usersService, _statusService);
        var validationResult = await validator.ValidateAsync(execDateRm);
        if (validationResult.IsValid)
            return Result.Ok();
            
        return Result.Errors<string>(validationResult.Errors.Format());
    }
}