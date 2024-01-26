Сначала надо запускать Кавку, затем проект-продьюсер, а затем проекты-консамеры

Для того, чтобы опубликовать сообщение, надо вызвать метод Produce интерфейса IProducer, например
```csharp
    var kafkaUser = KafkaUser.CreateFromUser(newUser);
    await _producer.Produce(_userWasAddedTopic, newUser.Id, kafkaUser);
```

Запуск Кавки [подробнее](https://habr.com/ru/articles/496182/):
1. Сначала надо перейти в папку с Кафкой, например
```
    d:\Programs\kafka_2.13-3.6.1
```
2. Apache Kafka работает поверх сервиса ZooKeeper. ZooKeeper — это распределенный сервис конфигурирования и синхронизации, запускаем его
```bash
    .\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties
```

3. Запускаем сервер
```bash
    .\bin\windows\kafka-server-start.bat .\config\server.properties
```
Иногда падает ошибка:
```
    Shutdown broker because all log dirs in failed (kafka.log.LogManager)
```
надо менять папку с логами в server.properties [подробнее](https://stackoverflow.com/questions/51644409/kafka-broker-fails-because-all-log-dirs-have-failed):
```
    log.dirs=d:/Programs/kafka_2.13-3.6.1/kafka-logs1
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
