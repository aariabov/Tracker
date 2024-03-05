using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions.Kafka;

public class KafkaInstruction
{
    public int Id { get; }
    public string Name { get; }
    public string CreatorId { get; }
    public string ExecutorId { get; }
    public int StatusId { get; }
    public DateTime Deadline { get; }
    public DateTime? ExecDate { get; }

    private KafkaInstruction(int id, string name, string creatorId, string executorId, int statusId, DateTime deadline, DateTime? execDate)
    {
        Id = id;
        Name = name;
        CreatorId = creatorId;
        ExecutorId = executorId;
        StatusId = statusId;
        Deadline = deadline;
        ExecDate = execDate;
    }

    public static KafkaInstruction CreateFromInstruction(Instruction instruction)
    {
        return new KafkaInstruction(
            instruction.Id,
            instruction.Name,
            instruction.CreatorId,
            instruction.ExecutorId,
            instruction.StatusId,
            instruction.Deadline,
            instruction.ExecDate);
    }
}
