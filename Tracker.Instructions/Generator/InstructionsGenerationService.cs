using System.Diagnostics;
using Tracker.Instructions.RequestModels;
using Tracker.Users;

namespace Tracker.Instructions.Generator;

public class InstructionsGenerationService
{
    private readonly UsersService _usersService;
    private readonly InstructionGenerator _instructionGenerator;
    private readonly IInstructionsRepository _instructionsRepository;
    private readonly IInstructionsService _instructionsService;

    public InstructionsGenerationService(UsersService usersService,
        InstructionGenerator instructionGenerator,
        IInstructionsRepository instructionsRepository, 
        IInstructionsService instructionsService)
    {
        _usersService = usersService;
        _instructionGenerator = instructionGenerator;
        _instructionsRepository = instructionsRepository;
        _instructionsService = instructionsService;
    }

    public async Task RunJob(GenerationRm model)
    {
        var stopwatch = Stopwatch.StartNew();

        await _instructionsRepository.TruncateInstructions();
        
        var allUsers = await _usersService.GetUsersTreeAsync();
        var bosses = allUsers.Where(u => u.Children != null && u.Children.Any()).ToArray();

        var instructions = _instructionGenerator.GenerateForLoop(model.Total, bosses);
        foreach (var instruction in instructions)
        {
            _instructionsRepository.CreateInstruction(instruction);
        }

        await _instructionsRepository.SaveChangesAsync();
        await _instructionsService.RecalculateAllClosureTable();
        
        stopwatch.Stop();
        var elapsed = stopwatch.Elapsed;
        Console.WriteLine($"Generation time: {elapsed.Hours}h {elapsed.Minutes}m {elapsed.Seconds}s");
    }
}