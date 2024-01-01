Для того, чтобы опубликовать сообщение, надо вызвать метод Produce интерфейса IProducer, например
```csharp
    var kafkaUser = KafkaUser.CreateFromUser(newUser);
    await _producer.Produce(_userWasAddedTopic, newUser.Id, kafkaUser);
```

Чтобы отслеживать сообщения надо:
1. перейти в папку с Кафкой, например
```
    d:\Programs\kafka_2.13-3.6.1
```
2. запустить команду, указав интересующий топик
```bash
    .\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic user_was_updated
```
~~~~