### Как поднять docker контейнер и запустить интеграционные тесты 
1. Открыть терминал
2. Перейти в Tracker
3. Поднять docker контейнер:
```bash
docker-compose -f docker-compose.tests.yml up
```
4. Запустить тесты из проекта Tracker.IntegrationTests.Docker
5. Остановить контейнер:
```bash
docker-compose -f docker-compose.tests.yml stop
```