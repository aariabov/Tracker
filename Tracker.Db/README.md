## Миграции

Все команды можно выполнить утилитой make, смотри Makefile

### Накатить миграцию:
1. Открыть терминал
2. Просмотреть список миграций
```bash
dotnet ef migrations list --context AppDbContext --project .\Tracker.Web
dotnet ef migrations list --context AnalyticsDbContext --project .\Tracker.Analytics
dotnet ef migrations list --context AuditDbContext --project .\Tracker.Audit
dotnet ef migrations list --context InstructionsDbContext --project .\Tracker.Instructions
```
4. Создать миграцию, например:
```bash
dotnet ef migrations add MIGRATION_NAME --context AuditDbContext --project .\Tracker.Audit --output-dir Db\Migrations
dotnet ef migrations add MIGRATION_NAME --context AnalyticsDbContext --project .\Tracker.Analytics --output-dir Db\Migrations
dotnet ef migrations add MIGRATION_NAME --context InstructionsDbContext --project .\Tracker.Instructions --output-dir Db\Migrations
```
или для Tracker.Db
```bash
dotnet ef migrations add MIGRATION_NAME --context AppDbContext --project .\Tracker.Db --startup-project .\Tracker.Web
```
Иногда возникает проблема типа
```
An item with the same key has already been added. Key: [0, Property: Tracker.Db.Models.Instruction (Dictionary<string, object>).ExecutorId (no field, string) Indexer Required FK Index]
```
Видимо, возникает, из-за ключей https://stackoverflow.com/questions/56144823/entity-framework-core-add-migration-throws-an-item-with-the-same-key-has-alread

Можно сначала создать миграцию на удаление ключей, а потом на удаление сущности.
Или не парится и написать миграцию самому https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/?redirectedfrom=MSDN#customizing-migrations
5. Проверить новый файл с миграцией, и посмотреть скрипт:
```bash
dotnet ef migrations script PREVIOUS_MIGRATION_NAME --context AppDbContext --project ..\Tracker.Db
dotnet ef migrations script InitTrackerInstructions --context InstructionsDbContext --project .\Tracker.Instructions
```
6. Если все Ок, накатить миграцию:
```bash
dotnet ef database update --context InstructionsDbContext --project .\Tracker.Instructions
```
   

### Откатить миграцию:
1. Открыть терминал
2. Перейти в Tracker.Web
3. Запустить команду
```bash
dotnet ef database update PREVIOUS_MIGRATION_NAME --context AppDbContext --project ..\Tracker.Db
dotnet ef database update InitTrackerInstructions --context InstructionsDbContext --project .\Tracker.Instructions
```
4. Удалить cs файл миграции
5. Откатить AppDbContextModelSnapshot


## Полезные sql запросы

Удалить все поручения:

```sql
TRUNCATE instructions RESTART IDENTITY CASCADE;
```


## Транзакции

Для работы с транзакциями используется ITransactionManager
```csharp
    using var transaction = _transactionManager.BeginTransaction();
    try
    {
        ...
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
```