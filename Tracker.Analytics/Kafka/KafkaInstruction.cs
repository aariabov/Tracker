using Tracker.Analytics.Db.Models;

namespace Tracker.Analytics.Kafka;

public class KafkaInstruction
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CreatorId { get; set; }
    public string ExecutorId { get; set; }
    public int StatusId { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? ExecDate { get; set; }


    public Instruction ToInstruction()
    {
        return new Instruction
        {
            Id = this.Id,
            Name = this.Name,
            CreatorId = this.CreatorId,
            ExecutorId = this.ExecutorId,
            StatusId = (ExecStatus)this.StatusId,
            Deadline = this.Deadline,
            ExecDate = this.ExecDate
        };
    }
}
