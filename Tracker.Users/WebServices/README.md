## Генерация кода по апи

1. Установить NSwag https://github.com/RicoSuter/NSwag/wiki/CommandLine
2. Создать схему генерации [name].nswag руками или через студию
3. Убедится, что запушен сервис, для того, что бы получить swagger.json
4. Запустить команду
```bash
dotnet-nswag run [name].nswag
```

### Сгенерировать код для Сервиса аудита:
```bash
dotnet-nswag run Tracker.Users/WebServices/Audit/generation-schema.nswag
```
