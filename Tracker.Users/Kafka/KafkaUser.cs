using Tracker.Db.Models;

namespace Tracker.Users.Kafka;

public class KafkaUser
{
    public string Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string? BossId { get; }

    private KafkaUser(string id, string name, string email, string? bossId)
    {
        Id = id;
        Name = name;
        Email = email;
        BossId = bossId;
    }

    public static KafkaUser CreateFromUser(User user)
    {
        return new KafkaUser(user.Id, user.UserName, user.Email, user.BossId);
    }
}
