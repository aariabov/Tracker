using System.Text.Json;
using Confluent.Kafka;

namespace Tracker.Instructions.Kafka;

public class KafkaUserConsumer : BackgroundService
{
    private readonly string _userWasUpdatedTopic;
    private readonly string _userWasAddedTopic;
    private readonly string _userWasDeletedTopic;
    private readonly IConsumer<Ignore, string> kafkaConsumer;
    private readonly IServiceProvider _serviceProvider;

    public KafkaUserConsumer(IConfiguration config, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var consumerConfig = new ConsumerConfig();
        config.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);

        _userWasUpdatedTopic = config.GetValue<string>("Kafka:UserWasUpdatedTopic");
        _userWasAddedTopic = config.GetValue<string>("Kafka:UserWasAddedTopic");
        _userWasDeletedTopic = config.GetValue<string>("Kafka:UserWasDeletedTopic");

        this.kafkaConsumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
    }

    private async Task StartConsumerLoop(CancellationToken cancellationToken)
    {
        var topics = new[] { this._userWasAddedTopic, this._userWasUpdatedTopic, this._userWasDeletedTopic };
        kafkaConsumer.Subscribe(topics);
        //kafkaConsumer.Subscribe(this._userWasDeletedTopic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = this.kafkaConsumer.Consume(cancellationToken);
                var payload = consumeResult.Message.Value;

                using var scope = _serviceProvider.CreateScope();
                var userService = scope.ServiceProvider.GetRequiredService<UserService>();
                if (consumeResult.Topic == _userWasUpdatedTopic)
                {
                    var kafkaUser = JsonSerializer.Deserialize<KafkaUser>(payload)!;
                    await userService.UpdateUser(kafkaUser);
                }
                else if (consumeResult.Topic == _userWasAddedTopic)
                {
                    var kafkaUser = JsonSerializer.Deserialize<KafkaUser>(payload)!;
                    await userService.InsertUser(kafkaUser);
                }
                else if (consumeResult.Topic == _userWasDeletedTopic)
                {
                    var deletedUserId = JsonSerializer.Deserialize<string>(payload)!;
                    // TODO: подумать, что делать с удаленными юзерами, возможно ставить флаг
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException e)
            {
                // Consumer errors should generally be ignored (or logged) unless fatal.
                Console.WriteLine($"Consume error: {e.Error.Reason}");

                if (e.Error.IsFatal)
                {
                    // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e}");
                break;
            }
        }
    }

    public override void Dispose()
    {
        this.kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
        this.kafkaConsumer.Dispose();

        base.Dispose();
    }
}
