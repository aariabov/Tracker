namespace Tracker.Users;

public interface IProducer
{
    Task Produce(string topic, string key, object value);
}
