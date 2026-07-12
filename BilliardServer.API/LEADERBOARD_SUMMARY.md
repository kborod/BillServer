# Сервис таблицы лидеров - Краткое резюме

## Что было реализовано

Добавлен полнофункциональный сервис таблицы лидеров (Leaderboard Service) с использованием Redis для кэширования.

## Созданные файлы

### Core слой (BilliardServer.Core)
1. **`Dto/Leaderboard/LeaderboardEntryDto.cs`**
   - DTO для записи таблицы лидеров
   - Содержит информацию о пользователе, его рейтинге и позиции

2. **`Abstractions/ILeaderboardService.cs`**
   - Интерфейс сервиса таблицы лидеров
   - Методы: `GetTopLeaderboard()`, `RefreshLeaderboard()`, `GetUserRank()`

### Application слой (BilliardServer.Application)
1. **`Leaderboard/LeaderboardService.cs`**
   - Реализация сервиса таблицы лидеров
   - Наследует `ILeaderboardService` и `IHostedService`
   - Автоматически инициализируется при запуске сервера
   - Кэширует данные в Redis на 1 час

2. **`Leaderboard/LEADERBOARD_README.md`**
   - Подробная документация сервиса

### API слой (BilliardServer.API)
1. **`Controllers/LeaderboardController.cs`**
   - REST контроллер для доступа к таблице лидеров
   - Endpoints:
     - `GET /api/leaderboard/top` - получить топ лидеров
     - `GET /api/leaderboard/rank/{userId}` - получить позицию пользователя
     - `POST /api/leaderboard/refresh` - обновить таблицу (Admin only)

### Data Access слой (BilliardServer.DataAccess)
1. **Обновлен `Repositories/UsersRepository.cs`**
   - Добавлен метод `GetAllUsersForLeaderboard()`
   - Получает всех пользователей из БД для построения таблицы лидеров

2. **Обновлен `Abstractions/IUsersRepository.cs`**
   - Добавлена сигнатура метода `GetAllUsersForLeaderboard()`

### Конфигурационные файлы
1. **Обновлен `Program.cs`**
   - Добавлен импорт `using BilliardServer.Application.Leaderboard;`
   - Добавлена конфигурация Redis через `IConnectionMultiplexer`
   - Зарегистрирован `LeaderboardService` как синглтон и HostedService

2. **Обновлен `appsettings.json`**
   - Добавлена строка подключения к Redis: `"Redis": "localhost:6379"`

3. **Обновлен `BilliardServer.API.csproj`**
   - Добавлен пакет `StackExchange.Redis` v2.8.13

4. **Обновлен `BilliardServer.Application.csproj`**
   - Добавлены пакеты:
     - `StackExchange.Redis` v2.8.13
     - `Microsoft.Extensions.Hosting.Abstractions` v10.0.0

## Документация

1. **`REDIS_SETUP.md`** - инструкция по установке и настройке Redis
2. **`LEADERBOARD_EXAMPLES.md`** - примеры использования API на разных языках
3. **`LEADERBOARD_README.md`** - подробная документация сервиса

## Как это работает

### При запуске сервера

1. **Инициализация Redis**
   - `IConnectionMultiplexer` создается и регистрируется как синглтон
   - Подключение к Redis по адресу из `appsettings.json`

2. **Инициализация LeaderboardService**
   - Сервис запускается как `IHostedService`
   - Автоматически вызывает `RefreshLeaderboard()`
   - Загружает всех пользователей из БД
   - Сортирует по рейтингу и количеству побед
   - Кэширует в Redis на 1 час

### При запросе к API

1. **GET /api/leaderboard/top**
   - Получает данные из кэша Redis
   - Если кэш пуст, обновляет его
   - Возвращает топ N пользователей

2. **GET /api/leaderboard/rank/{userId}**
   - Получает позицию пользователя в таблице
   - Возвращает -1 если пользователь не найден

3. **POST /api/leaderboard/refresh**
   - Требует роль Admin
   - Принудительно обновляет кэш
   - Загружает свежие данные из БД

## Сортировка

Пользователи сортируются по:
1. **Рейтинг** (Rating) - по убыванию (основной критерий)
2. **Побед** (WinPartiesCount) - по убыванию (вторичный критерий)

## Требования

- .NET 10.0
- Redis (локально или удаленно)
- PostgreSQL БД с полями в таблице Users:
  - `Rating` - рейтинг пользователя
  - `WinPartiesCount` - количество побед
  - `PartiesCount` - всего матчей
  - `Name` - имя пользователя

## Использование

### Простой пример на JavaScript

```javascript
// Получить топ 100 лидеров
const response = await fetch('/api/leaderboard/top?limit=100');
const leaderboard = await response.json();
console.log(leaderboard);

// Получить позицию пользователя
const rankResponse = await fetch('/api/leaderboard/rank/123');
const { rank } = await rankResponse.json();
console.log(`Ранг: ${rank}`);
```

## Тестирование

1. Убедиться, что Redis запущен
2. Запустить приложение: `dotnet run`
3. Проверить логи на успешную инициализацию:
   ```
   [INF] Initializing leaderboard service...
   [INF] Leaderboard refreshed successfully with X entries
   [INF] Leaderboard initialized successfully
   ```
4. Тестировать API через Swagger или cURL

## Возможные улучшения в будущем

1. Добавить фоновый сервис для автоматического обновления каждый час
2. Добавить региональные таблицы лидеров
3. Добавить историю изменения позиций
4. Добавить фильтрацию по времени (неделя, месяц, год)
5. Добавить минимальное количество матчей для попадания в таблицу
6. Оптимизировать хранение данных в Redis (использовать sorted sets)

## Тестирование производительности

- Кэш обновляется раз в час
- Максимальный размер ответа: 1000 записей
- Память Redis зависит от количества пользователей (~1KB на запись)
- Время ответа: <100ms для запросов из кэша

## Дополнительные команды

```bash
# Проверить работу Redis
redis-cli ping

# Посмотреть данные в кэше
redis-cli GET leaderboard:top

# Очистить кэш
redis-cli DEL leaderboard:top

# Запустить приложение
dotnet run

# Обновить таблицу лидеров через API
curl -X POST "http://localhost:5000/api/leaderboard/refresh" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```
