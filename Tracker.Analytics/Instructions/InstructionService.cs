using Tracker.Analytics.Kafka;

namespace Tracker.Analytics.Instructions;

public class InstructionService
{
    private readonly InstructionRepository _instructionRepository;

    public InstructionService(InstructionRepository instructionRepository)
    {
        _instructionRepository = instructionRepository;
    }

    public async Task UpdateInstruction(KafkaInstruction kafkaInstruction)
    {
        var instruction = kafkaInstruction.ToInstruction();
        _instructionRepository.UpdateInstruction(instruction);
        await _instructionRepository.SaveChangesAsync();
    }

    public async Task InsertInstruction(KafkaInstruction kafkaInstruction)
    {
        var instruction = kafkaInstruction.ToInstruction();
        _instructionRepository.InsertInstruction(instruction);
        await _instructionRepository.SaveChangesAsync();
    }
}
