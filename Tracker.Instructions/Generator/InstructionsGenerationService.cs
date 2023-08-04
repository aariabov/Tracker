using System.Diagnostics;
using Tracker.Instructions.RequestModels;

namespace Tracker.Instructions.Generator;

public class InstructionsGenerationService
{
    private readonly InstructionGenerator _instructionGenerator;
    private readonly IInstructionsRepository _instructionsRepository;
    private readonly IInstructionsService _instructionsService;
    private readonly UserRepository _userRepository;

    public InstructionsGenerationService(InstructionGenerator instructionGenerator,
        IInstructionsRepository instructionsRepository,
        IInstructionsService instructionsService,
        UserRepository userRepository)
    {
        _instructionGenerator = instructionGenerator;
        _instructionsRepository = instructionsRepository;
        _instructionsService = instructionsService;
        _userRepository = userRepository;
    }

    public async Task RunJob(GenerationRm model)
    {
        var stopwatch = Stopwatch.StartNew();

        await _instructionsRepository.TruncateInstructions();

        var allUsers = await _userRepository.GetAllUsers();
        var bosses = allUsers.Where(u => u.Children.Any()).ToArray();

        var instructions = _instructionGenerator.GenerateForLoop(model.Total, bosses.ToArray());
        foreach (var instruction in instructions)
        {
            _instructionsRepository.CreateInstruction(instruction);
        }

        await _instructionsRepository.SaveChangesAsync();
        await _instructionsService.RecalculateAllClosureTable();
        await _instructionsRepository.UpdateSequence();

        stopwatch.Stop();
        var elapsed = stopwatch.Elapsed;
        Console.WriteLine($"Generation time: {elapsed.Hours}h {elapsed.Minutes}m {elapsed.Seconds}s");
    }
}
