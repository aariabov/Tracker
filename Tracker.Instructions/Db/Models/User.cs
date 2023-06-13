namespace Tracker.Instructions.Db.Models;

public class User
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string? BossId { get; set; }
    public User? Boss { get; set; }
    public ICollection<User> Children { get; set; } = new List<User>();
}
