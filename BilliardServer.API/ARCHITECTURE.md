# 🏗️ Архитектура Leaderboard Service

## Диаграмма компонентов

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                             │
│                                                                   │
│  ┌──────────────────┐  ┌──────────────────┐  ┌────────────────┐ │
│  │  Web Browser     │  │  Mobile App      │  │  External API  │ │
│  └────────┬─────────┘  └────────┬─────────┘  └────────┬────────┘ │
└───────────┼──────────────────────┼──────────────────────┼──────────┘
            │ HTTP Requests        │                      │
┌───────────▼──────────────────────▼──────────────────────▼──────────┐
│                      API LAYER (Controllers)                        │
│  ┌───────────────────────────────────────────────────────────────┐ │
│  │ LeaderboardController                                         │ │
│  │ ├─ GET  /api/leaderboard/top                                 │ │
│  │ ├─ GET  /api/leaderboard/rank/{userId}                       │ │
│  │ └─ POST /api/leaderboard/refresh (Admin only)                │ │
│  └─────────────┬──────────────────────────────────────────────┬─┘ │
└────────────────┼──────────────────────────────────────────────┼────┘
                 │ Dependency Injection                         │
┌────────────────▼──────────────────────────────────────────────▼────┐
│                 APPLICATION LAYER (Services)                        │
│  ┌───────────────────────────────────────────────────────────────┐ │
│  │ LeaderboardService : ILeaderboardService, IHostedService     │ │
│  │                                                               │ │
│  │ ┌─────────────────────────────────────────────────────────┐ │ │
│  │ │ Methods:                                                │ │ │
│  │ │ • GetTopLeaderboard(limit)                              │ │ │
│  │ │ • GetUserRank(userId)                                   │ │ │
│  │ │ • RefreshLeaderboard()                                  │ │ │
│  │ │ • StartAsync() / StopAsync() [IHostedService]           │ │ │
│  │ └─────────────────────────────────────────────────────────┘ │ │
│  │                                                               │ │
│  │ [Runs automatically on startup]                              │ │
│  └─────────────┬──────────────────────────────────────────────┬─┘ │
└────────────────┼──────────────────────────────────────────────┼────┘
                 │ Depends on                                   │
           ┌─────▼──────────────────────────────────────┬──────▼─┐
           │                                            │         │
┌──────────▼────────────────────┐        ┌──────────────▼───────┐
│  CORE LAYER (Abstractions)    │        │  DATA ACCESS LAYER   │
│                               │        │                      │
│ ┌─────────────────────────────┤        │ ┌──────────────────┐ │
│ │ ILeaderboardService         │        │ │ IUsersRepository │ │
│ │ ├─ GetTopLeaderboard()      │        │ ├─ GetAllUsersFor │ │
│ │ ├─ GetUserRank()            │        │ │   Leaderboard() │ │
│ │ └─ RefreshLeaderboard()     │        │ └────────┬─────────┘ │
│ └─────────────────────────────┤        │          │            │
│                               │        │ ┌────────▼─────────┐ │
│ ┌─────────────────────────────┤        │ │ UsersRepository  │ │
│ │ LeaderboardEntryDto         │        │ │ (Implementation) │ │
│ │ • UserId                    │        │ └────────┬─────────┘ │
│ │ • UserName                  │        │          │            │
│ │ • Rating                    │        └──────────┼────────────┘
│ │ • WinPartiesCount           │                   │
│ │ • PartiesCount              │        ┌──────────▼────────────┐
│ │ • Rank                      │        │   DATABASE LAYER     │
│ └─────────────────────────────┘        │                      │
└──────────────────────────────────────┐ │ ┌──────────────────┐ │
                                       │ │ │  PostgreSQL DB   │ │
                    ┌──────────────────┼─┼─┤  Users Table     │ │
                    │ Caching          │ │ │  ├─ Id           │ │
                    │                  │ │ │  ├─ Name         │ │
            ┌───────▼────────────┐    │ │ │  ├─ Rating       │ │
            │   REDIS CACHE      │    │ │ │  ├─ WinCount     │ │
            │                    │    │ │ │  ├─ PartiesCount │ │
            │ Key:               │    │ │ │  └─ ...          │ │
            │ leaderboard:top    │    │ │ │                  │ │
            │                    │    │ │ └──────────────────┘ │
            │ Value:             │    │ │                      │
            │ [JSON Array]       │    │ └──────────────────────┘
            │                    │    │
            │ TTL: 1 hour        │    │
            └────────────────────┘    │
                                      │
                    ┌─────────────────▼──────────────────────┐
                    │  EXTERNAL SERVICES                     │
                    │                                        │
                    │ • Redis (Caching)                      │
                    │ • PostgreSQL (Data Persistence)        │
                    │ • Logger (Diagnostics)                 │
                    │ • DependencyInjection (IoC)            │
                    └────────────────────────────────────────┘
```

## Последовательность запросов

### 1. Получение топ лидеров

```
Client
  │
  └─> GET /api/leaderboard/top?limit=50
        │
        ▼
  LeaderboardController.GetTopLeaderboard(50)
        │
        ▼
  ILeaderboardService.GetTopLeaderboard(50)
        │
        ▼
  Redis.StringGetAsync("leaderboard:top")
        │
        ├─ [HIT] Cache exists
        │   │
        │   └─> Deserialize JSON
        │       │
        │       └─> Take(50)
        │           │
        │           └─> Return 50 entries ✓
        │
        └─ [MISS] Cache empty
            │
            └─> RefreshLeaderboard()
                │
                ├─> IUsersRepository.GetAllUsersForLeaderboard()
                │   │
                │   └─> SELECT * FROM Users
                │       │
                │       └─> Map to User objects
                │
                ├─> Sort by Rating DESC, WinPartiesCount DESC
                │
                ├─> Assign ranks (1, 2, 3, ...)
                │
                ├─> Serialize to JSON
                │
                ├─> Redis.StringSetAsync("leaderboard:top", json, 1hour)
                │
                └─> Take(50) and Return ✓
```

### 2. При запуске приложения

```
Application Startup
  │
  └─> Program.Main()
        │
        ├─> Register all services
        │   ├─> IConnectionMultiplexer (Redis)
        │   ├─> ILeaderboardService (singleton)
        │   └─> LeaderboardService as IHostedService
        │
        ├─> app.Run()
        │   │
        │   └─> Start all IHostedService instances
        │       │
        │       └─> LeaderboardService.StartAsync()
        │           │
        │           ├─> Log: "Initializing leaderboard service..."
        │           │
        │           ├─> RefreshLeaderboard()
        │           │   ├─> Get all users from DB
        │           │   ├─> Sort and assign ranks
        │           │   ├─> Cache in Redis
        │           │   └─> Log: "Leaderboard refreshed with X entries"
        │           │
        │           └─> Log: "Leaderboard initialized successfully"
        │
        └─> Application ready for requests ✓
```

## Data Flow

```
┌─────────────────────────────────────────────────────────────┐
│                     CLIENT REQUEST                           │
│                  GET /api/leaderboard/top                   │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
        ┌────────────────────────────────┐
        │   LeaderboardController        │
        │   .GetTopLeaderboard(limit)    │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  ILeaderboardService           │
        │  .GetTopLeaderboard(limit)     │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │   Check Redis Cache            │
        └────────┬───────────────┬───────┘
                 │               │
        ┌────────▼┐         ┌────▼────────┐
        │  CACHE  │         │   MISS      │
        │   HIT   │         │  (Empty)    │
        └────────┬┘         └────┬────────┘
                 │               │
         ┌───────▼┐      ┌──────▼───────┐
         │Deserialize│   │Query Database │
         │           │   │  Get Users    │
         └───────┬───┘   └──────┬────────┘
                 │              │
                 │      ┌───────▼───────┐
                 │      │ Process Data  │
                 │      │ - Sort        │
                 │      │ - Rank        │
                 │      │ - Serialize   │
                 │      └───────┬───────┘
                 │              │
                 │      ┌───────▼───────┐
                 │      │ Cache in Redis│
                 │      │ TTL: 1 hour   │
                 │      └───────┬───────┘
                 │              │
                 └──────┬───────┘
                        │
                 ┌──────▼──────────┐
                 │  Take(limit)    │
                 │  Filter results │
                 └──────┬──────────┘
                        │
                        ▼
        ┌────────────────────────────────┐
        │  LeaderboardEntryDto[] (JSON)  │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │      HTTP 200 Response         │
        │  Content-Type: application/json│
        └────────────────────────────────┘
```

## Классы и их взаимодействие

```
┌─────────────────────────────────────────────────────────────┐
│ LeaderboardController                                        │
│                                                             │
│ - _leaderboardService: ILeaderboardService                 │
│ - _logger: ILogger<LeaderboardController>                  │
│                                                             │
│ Methods:                                                   │
│ + GetTopLeaderboard(limit) : Task<List<LeaderboardEntry>> │
│ + GetUserRank(userId) : Task<int>                          │
│ + RefreshLeaderboard() : Task                              │
└────────────────┬────────────────────────────────────────────┘
                 │ uses
                 ▼
┌─────────────────────────────────────────────────────────────┐
│ ILeaderboardService (Interface)                             │
│                                                             │
│ Methods:                                                   │
│ + GetTopLeaderboard(limit) : Task<List<LeaderboardEntry>> │
│ + RefreshLeaderboard() : Task                              │
│ + GetUserRank(userId) : Task<int>                          │
└─────────────────────────────────────────────────────────────┘
         △
         │ implements
         │
┌────────┴────────────────────────────────────────────────────┐
│ LeaderboardService : ILeaderboardService, IHostedService   │
│                                                             │
│ Private Fields:                                            │
│ - _redis: IConnectionMultiplexer                           │
│ - _usersRepository: IUsersRepository                       │
│ - _logger: ILogger<LeaderboardService>                     │
│                                                             │
│ Methods:                                                   │
│ + StartAsync() : Task                                      │
│ + StopAsync() : Task                                       │
│ + GetTopLeaderboard(limit) : Task<List<...>>              │
│ + RefreshLeaderboard() : Task                              │
│ + GetUserRank(userId) : Task<int>                          │
│ - LoadFromRedis() : List<LeaderboardEntry>                │
│ - SaveToRedis(data) : Task                                │
└────────┬──────────────────────────────┬──────────────────┬──┘
         │ uses                         │ uses             │
         ▼                              ▼                  ▼
    ┌─────────────┐        ┌────────────────────┐    ┌─────────┐
    │   Redis     │        │ IUsersRepository   │    │ ILogger │
    │  IConnection           │                    │    └─────────┘
    │ Multiplexer │        │ + GetAllUsersFor   │
    │             │        │   Leaderboard()    │
    └─────────────┘        └────────────────────┘
         │                          △
         │ stores/retrieves         │ uses
         │                          │
         ▼                          ▼
    ┌──────────────┐    ┌─────────────────────┐
    │  Redis Cache │    │ UsersRepository     │
    │              │    │ : IUsersRepository  │
    │ Key:         │    │                     │
    │ leaderboard  │    │ Methods:            │
    │ :top         │    │ + GetAllUsersFor... │
    │              │    │ + GetUser()         │
    │ TTL: 1 hour  │    │ + GetByEmail()      │
    └──────────────┘    │ + ...               │
                        └──────────┬──────────┘
                                   │ queries
                                   ▼
                        ┌──────────────────┐
                        │  PostgreSQL DB   │
                        │  Users Table     │
                        └──────────────────┘
```

## Сортировка и Ранжирование

```
Raw Data from DB:
┌────────────────────────────────┐
│ UserID │ Name    │ Rating │ Win │
├────────┼─────────┼────────┼─────┤
│ 1      │ Player1 │ 4800   │ 140 │
│ 2      │ Player2 │ 5000   │ 250 │
│ 3      │ Player3 │ 4600   │ 130 │
│ 4      │ Player4 │ 5000   │ 240 │
│ 5      │ Player5 │ 4700   │ 135 │
└────────────────────────────────┘
         │
         ▼
Sort by: Rating DESC, WinCount DESC
         │
         ▼
Sorted Data:
┌────────────────────────────────────────┐
│ Rank │ UserID │ Name    │ Rating │ Win │
├──────┼────────┼─────────┼────────┼─────┤
│ 1    │ 2      │ Player2 │ 5000   │ 250 │ ← Highest Rating, Most Wins
│ 2    │ 4      │ Player4 │ 5000   │ 240 │ ← Same Rating, Fewer Wins
│ 3    │ 1      │ Player1 │ 4800   │ 140 │
│ 4    │ 5      │ Player5 │ 4700   │ 135 │
│ 5    │ 3      │ Player3 │ 4600   │ 130 │ ← Lowest Rating
└────────────────────────────────────────┘
         │
         ▼
Cache as JSON:
[
  {"userId": 2, "userName": "Player2", "rating": 5000, "rank": 1, ...},
  {"userId": 4, "userName": "Player4", "rating": 5000, "rank": 2, ...},
  ...
]
```

## Кэш управление

```
┌─────────────────────────────────────────┐
│     Cache Lifecycle                     │
└─────────────────────────────────────────┘

Start of Day:
    ├─ 00:00 - Application starts
    │   └─ RefreshLeaderboard() called
    │       └─ Data cached for 1 hour
    │
    ├─ 00:00-00:55 - Cache HIT
    │   └─ Return cached data (<100ms)
    │
    └─ 01:00 - Cache MISS (TTL expired)
        └─ Refresh from DB
            └─ Cache for next hour

Manual Refresh:
    └─ Admin calls POST /api/leaderboard/refresh
        └─ Force update cache immediately
            └─ TTL resets to 1 hour
```

---

Эта диаграмма показывает полную архитектуру сервиса таблицы лидеров и как все компоненты взаимодействуют друг с другом.
