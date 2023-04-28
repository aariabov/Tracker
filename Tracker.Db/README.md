## Миграции

### Накатить миграцию:
1. Открыть терминал
2. Перейти в Tracker.Web
3. Просмотреть список миграций
```bash
dotnet ef migrations list --context AppDbContext --project ..\Tracker.Db
```
4. Создать миграцию, например:
```bash
dotnet ef migrations add MIGRATION_NAME --context AuditDbContext --project ..\Tracker.Audit --output-dir Db\Migrations
```
или для Tracker.Db
```bash
dotnet ef migrations add MIGRATION_NAME --context AppDbContext --project ..\Tracker.Db
```
5. Проверить новый файл с миграцией, и посмотреть скрипт:
```bash
dotnet ef migrations script PREVIOUS_MIGRATION_NAME --context AppDbContext --project ..\Tracker.Db
```
6. Если все Ок, накатить миграцию:
```bash
dotnet ef database update --context AppDbContext --project ..\Tracker.Db
```
   

### Откатить миграцию:
1. Открыть терминал
2. Перейти в Tracker.Web
3. Запустить команду
```bash
dotnet ef database update PREVIOUS_MIGRATION_NAME --context AppDbContext --project ..\Tracker.Db
```
4. Удалить cs файл миграции
5. Откатить AppDbContextModelSnapshot


## Полезные sql запросы

Удалить все поручения:

```sql
TRUNCATE instructions RESTART IDENTITY CASCADE;
```