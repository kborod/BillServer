# 🏆 Billiard Server - Leaderboard Service

Полнофункциональный сервис таблицы лидеров с кэшированием в Redis для приложения Billiard Server.

## 🎯 Особенности

✅ **Автоматическая инициализация** - запускается при старте сервера  
✅ **Redis кэширование** - данные кэшируются на 1 час  
✅ **REST API** - 3 удобных endpoints для работы с лидерами  
✅ **Асинхронная обработка** - полностью асинхронный код  
✅ **Безопасность** - защищенный refresh endpoint  
✅ **Логирование** - детальное логирование операций  
✅ **Масштабируемость** - готово к использованию в production  

## 🚀 Быстрый старт

### 1️⃣ Запустить Redis

```bash
# Docker (рекомендуется)
docker run -d -p 6379:6379 --name redis redis:latest

# Проверить
redis-cli ping  # PONG
```

### 2️⃣ Запустить приложение

```bash
cd BilliardServer.API
dotnet run
```

Должны увидеть логи:
```
[INF] Initializing leaderboard service...
[INF] Leaderboard refreshed successfully with X entries
[INF] Leaderboard initialized successfully
```

### 3️⃣ Тестировать API

```bash
# Получить топ 10 лидеров
curl http://localhost:5000/api/leaderboard/top?limit=10

# Получить рейтинг пользователя
curl http://localhost:5000/api/leaderboard/rank/1

# Обновить таблицу (требует Admin токен)
curl -X POST http://localhost:5000/api/leaderboard/refresh \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 📋 API Endpoints

### GET /api/leaderboard/top

Получить топ таблицу лидеров

```bash
curl "http://localhost:5000/api/leaderboard/top?limit=50"
```

**Параметры:**
- `limit` (optional): Количество записей (1-1000, default 100)

**Ответ:**
```json
[
  {
    "userId": 1,
    "userName": "Champion",
    "rating": 5000,
    "winPartiesCount": 250,
    "partiesCount": 300,
    "rank": 1
  },
  ...
]
```

### GET /api/leaderboard/rank/{userId}

Получить позицию пользователя в таблице

```bash
curl "http://localhost:5000/api/leaderboard/rank/123"
```

**Ответ:**
```json
{
  "rank": 5
}
```

### POST /api/leaderboard/refresh

Обновить таблицу лидеров (требует роль Admin)

```bash
curl -X POST "http://localhost:5000/api/leaderboard/refresh" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Ответ:**
```json
{
  "message": "Leaderboard refreshed successfully"
}
```

## 📊 Технические детали

| Параметр | Значение |
|----------|----------|
| Язык | C# (.NET 10.0) |
| Кэш | Redis |
| TTL кэша | 1 час |
| Хранилище | PostgreSQL |
| Сортировка | Rating DESC, Wins DESC |
| Максимум записей | 1000 |
| Время ответа | <100ms (из кэша) |

## 📁 Структура проекта

```
BilliardServer/
├── BilliardServer.API/
│   ├── Controllers/LeaderboardController.cs      [REST API]
│   ├── Program.cs                                 [Configuration]
│   └── appsettings.json                          [Settings]
│
├── BilliardServer.Application/
│   └── Leaderboard/LeaderboardService.cs         [Main Service]
│
├── BilliardServer.Core/
│   ├── Abstractions/ILeaderboardService.cs       [Interface]
│   └── Dto/Leaderboard/LeaderboardEntryDto.cs    [Model]
│
└── BilliardServer.DataAccess/
    ├── Repositories/UsersRepository.cs           [Data Access]
    └── Abstractions/IUsersRepository.cs          [Interface]
```

## 🔧 Конфигурация

### appsettings.json

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

Для удаленного Redis:
```json
{
  "ConnectionStrings": {
    "Redis": "your-redis-server.com:6379"
  }
}
```

## 📚 Документация

Подробная документация находится в следующих файлах:

- **[QUICK_START.md](./QUICK_START.md)** - Быстрый старт за 5 минут ⭐
- **[IMPLEMENTATION_DETAILS.md](./IMPLEMENTATION_DETAILS.md)** - Полная реализация
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Диаграммы архитектуры
- **[LEADERBOARD_EXAMPLES.md](./LEADERBOARD_EXAMPLES.md)** - Примеры кода
- **[REDIS_SETUP.md](./REDIS_SETUP.md)** - Установка Redis
- **[SUMMARY_OF_CHANGES.md](./SUMMARY_OF_CHANGES.md)** - Все изменения

## 💻 Примеры использования

### JavaScript

```javascript
// Получить топ лидеров
const response = await fetch('/api/leaderboard/top?limit=100');
const leaderboard = await response.json();

// Получить рейтинг
const rankResponse = await fetch('/api/leaderboard/rank/123');
const { rank } = await rankResponse.json();
```

### Python

```python
import requests

# Получить топ лидеров
response = requests.get('http://localhost:5000/api/leaderboard/top?limit=100')
leaderboard = response.json()
```

### cURL

```bash
# Топ 50
curl http://localhost:5000/api/leaderboard/top?limit=50

# Рейтинг пользователя
curl http://localhost:5000/api/leaderboard/rank/1
```

## 🧪 Тестирование

```bash
# 1. Проверить Redis
redis-cli ping

# 2. Запустить приложение
dotnet run

# 3. Тестировать API
curl http://localhost:5000/api/leaderboard/top?limit=10

# 4. Проверить кэш в Redis
redis-cli GET leaderboard:top
```

## ⚙️ Требования

- **.NET 10.0** - Runtime и SDK
- **Redis** - для кэширования
- **PostgreSQL** - для данных пользователей
- **StackExchange.Redis** - NuGet пакет (уже добавлен)

## 🔄 Жизненный цикл данных

```
Запуск сервера
    ↓
LeaderboardService.StartAsync()
    ↓
RefreshLeaderboard()
    ├─ Загрузить пользователей из БД
    ├─ Отсортировать по рейтингу
    ├─ Присвоить ранги
    └─ Сохранить в Redis (TTL = 1 час)
    ↓
Готово к запросам
    ↓
Клиент запрашивает данные → Redis (быстро <100ms)
    ↓
Кэш устаревает через 1 час
    ↓
Автоматическое обновление или вручную через /refresh
```

## 🐛 Отладка

### Redis не подключается

```bash
# Проверить, что Redis запущен
redis-cli ping

# Проверить строку подключения в appsettings.json
# Проверить firewall настройки
```

### Пусто в таблице

```bash
# Убедиться, что в БД есть пользователи с Rating > 0
# Вызвать вручную: POST /api/leaderboard/refresh
# Проверить логи приложения
```

### Медленный ответ

```bash
# Кэш может быть пуст, дождитесь обновления
# Или вызовите POST /api/leaderboard/refresh вручную
```

## 📈 Мониторинг

### Redis Commander

```bash
# Установить
npm install -g redis-commander

# Запустить
redis-commander

# Открыть http://localhost:8081
```

### Проверить кэш

```bash
redis-cli
GET leaderboard:top
INFO memory
```

## 🚀 Production развертывание

1. Обновить `appsettings.Production.json` с правильной Redis строкой
2. Убедиться, что Redis доступен
3. Запустить `dotnet publish -c Release`
4. Развернуть сборку на сервер
5. Запустить приложение

## 📝 Логирование

Включить debug логирование:

```json
{
  "Logging": {
    "LogLevel": {
      "BilliardServer.Application.Leaderboard": "Debug"
    }
  }
}
```

## ✨ Особенности реализации

- ✅ Полностью асинхронный код
- ✅ Управление исключениями и ошибками
- ✅ Детальное логирование
- ✅ Оптимизированные запросы к БД
- ✅ Эффективное кэширование
- ✅ SOLID принципы
- ✅ Dependency Injection
- ✅ Unit-testable архитектура

## 🔐 Безопасность

- Endpoint `/refresh` требует роль **Admin**
- Используется JWT аутентификация
- Все операции логируются
- Входные данные валидируются

## 🎓 Изучение

Рекомендуемый порядок изучения:

1. **[QUICK_START.md](./QUICK_START.md)** - Начните отсюда
2. **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Поймите архитектуру
3. **[LEADERBOARD_EXAMPLES.md](./LEADERBOARD_EXAMPLES.md)** - Посмотрите примеры
4. **[IMPLEMENTATION_DETAILS.md](./IMPLEMENTATION_DETAILS.md)** - Углубленное изучение

## 📞 Поддержка

По вопросам смотрите документацию в следующих файлах:
- Как начать? → **QUICK_START.md**
- Как это работает? → **ARCHITECTURE.md**
- Как использовать? → **LEADERBOARD_EXAMPLES.md**
- Все детали → **IMPLEMENTATION_DETAILS.md**

## 📄 Лицензия

Часть проекта BilliardServer

## 👨‍💻 Разработчик

Создано как часть BilliardServer Project

---

## 🎉 Готово!

Таблица лидеров полностью интегрирована и готова к использованию!

```bash
# Просто запустите:
dotnet run
```

Приложение автоматически инициализирует таблицу лидеров при запуске.
