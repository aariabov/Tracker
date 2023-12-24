using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions.Kafka;

public class KafkaUser
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? BossId { get; set; }

    public User ToUser()
    {
        return new User
        {
            Id = this.Id,
            UserName = this.Name,
            BossId = this.BossId
        };
    }
}
