# Сервис Таблицы Лидеров (Leaderboard Service)

## Описание

Сервис таблицы лидеров реализует функционал отслеживания и кэширования статистики пользователей в Redis. Данные сортируются по рейтингу и количеству побед.

## Функциональность

### Компоненты

- **LeaderboardService** (`BilliardServer.Application/Leaderboard/LeaderboardService.cs`)
  - HostedService, который автоматически инициализирует таблицу лидеров при запуске сервера
  - Кэширует данные в Redis на 1 час

- **LeaderboardController** (`BilliardServer.API/Controllers/LeaderboardController.cs`)
  - REST API для доступа к таблице лидеров

## API Endpoints

### 1. Получить топ таблицу лидеров
```
GET /api/leaderboard/top?limit=100
```

**Параметры:**
- `limit` (query, optional): Количество записей для возврата (по умолчанию 100, максимум 1000)

**Ответ (200 OK):**
```json
[
  {
    "userId": 1,
    "userName": "Player1",
    "rating": 5000,
    "winPartiesCount": 150,
    "partiesCount": 200,
    "rank": 1
  },
  {
    "userId": 2,
    "userName": "Player2",
    "rating": 4800,
    "winPartiesCount": 140,
    "partiesCount": 195,
    "rank": 2
  }
]
```

### 2. Получить позицию пользователя в таблице
```
GET /api/leaderboard/rank/{userId}
```

**Параметры:**
- `userId` (path, required): ID пользователя

**Ответ (200 OK):**
```json
{
  "rank": 5
}
```

**Примечание:** Если пользователь не найден в таблице лидеров, возвращается `rank: -1`

### 3. Обновить таблицу лидеров (Admin only)
```
POST /api/leaderboard/refresh
```

**Требования:**
- Требуется роль "Admin"
- Требуется авторизация

**Ответ (200 OK):**
```json
{
  "message": "Leaderboard refreshed successfully"
}
```

## Кэширование в Redis

### Конфигурация

Строка подключения к Redis указывается в `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

### Ключи Redis

- `leaderboard:top` - основная таблица лидеров, хранится в виде JSON
- Время жизни кэша: 1 час

### Инициализация при запуске

При запуске сервера:
1. Сервис LeaderboardService инициализируется как IHostedService
2. Автоматически вызывается метод `RefreshLeaderboard()`
3. Загружаются все пользователи из БД
4. Данные сортируются по рейтингу и победам
5. Результат кэшируется в Redis

## Интеграция

### Зависимости

Пакеты в `BilliardServer.API.csproj`:
- `StackExchange.Redis` v2.8.13

Пакеты в `BilliardServer.Application.csproj`:
- `StackExchange.Redis` v2.8.13
- `Microsoft.Extensions.Hosting.Abstractions` v10.0.0

### Регистрация сервисов

В `Program.cs`:
```csharp
// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnection));

// LeaderboardService
builder.Services.AddSingleton<LeaderboardService>();
builder.Services.AddSingleton<ILeaderboardService>(sp => sp.GetRequiredService<LeaderboardService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<LeaderboardService>());
```

## Логирование

Сервис логирует:
- Инициализацию при запуске сервера
- Обновление таблицы лидеров
- Количество записей в таблице
- Ошибки при работе с Redis или БД

Примеры логов:
```
[INF] Initializing leaderboard service...
[INF] Leaderboard initialized successfully
[INF] Refreshing leaderboard...
[INF] Leaderboard refreshed successfully with 150 entries
[ERR] Error refreshing leaderboard: {exception details}
```

## Сортировка

Пользователи в таблице лидеров сортируются по:
1. **Рейтинг** (по убыванию) - основной критерий
2. **Количество побед** (по убыванию) - вторичный критерий

## Примеры использования

### cURL

```bash
# Получить топ 50 лидеров
curl -X GET "http://localhost:5000/api/leaderboard/top?limit=50"

# Получить рейтинг пользователя
curl -X GET "http://localhost:5000/api/leaderboard/rank/123"

# Обновить таблицу (требует токен с ролью Admin)
curl -X POST "http://localhost:5000/api/leaderboard/refresh" \
  -H "Authorization: Bearer <jwt_token>"
```

### JavaScript/TypeScript

```javascript
// Получить топ лидеров
const response = await fetch('/api/leaderboard/top?limit=100');
const leaderboard = await response.json();

// Получить рейтинг пользователя
const rankResponse = await fetch('/api/leaderboard/rank/123');
const { rank } = await rankResponse.json();
```

## Обновление таблицы лидеров

### Автоматическое обновление

После каждого матча, когда вызывается `UpdateAfterMatch` в репозитории, рекомендуется периодически обновлять таблицу лидеров. Текущая реализация использует кэширование на 1 час.

Для более частого обновления можно:
1. Вызвать эндпоинт `/api/leaderboard/refresh` (требует роль Admin)
2. Добавить фоновый сервис для периодического обновления

## Возможные улучшения

1. **Региональные таблицы лидеров** - кэшировать лидеров по регионам/группам
2. **История лидеров** - сохранять снимки таблицы лидеров с определенной периодичностью
3. **Личная статистика** - добавить эндпоинт для получения персональной статистики игрока
4. **Фоновое обновление** - добавить фоновый сервис для автоматического обновления каждый час
5. **Фильтрация** - добавить фильтры по времени, минимальному количеству матчей и т.д.

## Требования

- Redis должен быть доступен по адресу из `appsettings.json`
- Таблица `Users` в БД должна содержать поля: `Rating`, `WinPartiesCount`, `PartiesCount`
