namespace Tracker.Instructions.Kafka;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Confluent.Kafka;

public interface IProducer
{
    Task Produce(string topic, string key, object value);
}

public class KafkaProducer : IProducer
{
    private readonly IProducer<string, string> _kafkaHandle;
    private readonly JsonSerializerOptions _serializerOptions;

    public KafkaProducer(KafkaClientHandle handle)
    {
        _kafkaHandle = new DependentProducerBuilder<string, string>(handle.Handle).Build();
        _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
    }

    public Task Produce(string topic, string key, object value)
    {
        var valueStr = JsonSerializer.Serialize(value, _serializerOptions);

        var message = new Message<string, string>
        {
            Key = key,
            Value = valueStr
        };

        return _kafkaHandle.ProduceAsync(topic, message);
    }

    public void Flush(TimeSpan timeout) => _kafkaHandle.Flush(timeout);
}
