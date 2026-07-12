# 🚀 Быстрый старт - Leaderboard Service

## За 5 минут до работающей таблицы лидеров

### Шаг 1: Запустить Redis (2 минуты)

#### Docker (самый простой вариант)
```powershell
# Скачать и запустить Redis контейнер
docker run -d -p 6379:6379 --name redis redis:latest

# Проверить, что запущен
docker ps | grep redis
```

#### Или используя WSL/Linux
```bash
sudo systemctl start redis-server
redis-cli ping  # Должно вернуть PONG
```

### Шаг 2: Запустить приложение (1 минута)

```bash
cd D:\ProjectBilliardServer\BillServer\BilliardServer.API
dotnet run
```

**Вы должны увидеть в консоли:**
```
[INF] Initializing leaderboard service...
[INF] Refreshing leaderboard...
[INF] Leaderboard refreshed successfully with X entries
[INF] Leaderboard initialized successfully
```

### Шаг 3: Тестировать API (2 минуты)

#### Через Swagger UI
```
http://localhost:5000/swagger
```
Найдите секцию "Leaderboard" и тестируйте endpoints

#### Или через PowerShell
```powershell
# Получить топ 10 лидеров
$response = Invoke-RestMethod -Uri "http://localhost:5000/api/leaderboard/top?limit=10" -Method Get
$response | ConvertTo-Json

# Получить рейтинг пользователя
Invoke-RestMethod -Uri "http://localhost:5000/api/leaderboard/rank/1" -Method Get
```

#### Или через cURL
```bash
# Получить топ лидеров
curl http://localhost:5000/api/leaderboard/top?limit=10

# Получить рейтинг пользователя
curl http://localhost:5000/api/leaderboard/rank/1
```

---

## 📋 Что было добавлено

| Компонент | Файл | Описание |
|-----------|------|---------|
| Service | `BilliardServer.Application/Leaderboard/LeaderboardService.cs` | Основной сервис с кэшем Redis |
| Controller | `BilliardServer.API/Controllers/LeaderboardController.cs` | REST API endpoints |
| DTO | `BilliardServer.Core/Dto/Leaderboard/LeaderboardEntryDto.cs` | Модель данных |
| Interface | `BilliardServer.Core/Abstractions/ILeaderboardService.cs` | Контракт сервиса |
| Repository | Updated | Добавлен метод `GetAllUsersForLeaderboard()` |

---

## 🎯 Основные API endpoints

### 1. Получить топ лидеров
```
GET /api/leaderboard/top?limit=100
```
Возвращает список топ N пользователей, отсортированных по рейтингу

### 2. Получить позицию пользователя
```
GET /api/leaderboard/rank/123
```
Возвращает позицию конкретного пользователя в таблице

### 3. Обновить таблицу (Admin)
```
POST /api/leaderboard/refresh
Authorization: Bearer <token>
```
Принудительно обновляет кэш из БД (требует роль Admin)

---

## 🔍 Проверка работы

### Redis работает?
```bash
redis-cli ping
# Должно вернуть: PONG
```

### Данные в кэше?
```bash
redis-cli GET leaderboard:top | head -100
# Должен вернуть JSON с данными лидеров
```

### Приложение работает?
```bash
# Все три должны вернуть JSON ответы
curl http://localhost:5000/api/leaderboard/top?limit=5
curl http://localhost:5000/api/leaderboard/rank/1
```

---

## ⚙️ Конфигурация

### Строка подключения Redis
**Файл**: `appsettings.json`
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

---

## 📊 Данные таблицы

Каждая запись содержит:
```json
{
  "userId": 1,           // ID пользователя
  "userName": "Player1", // Имя пользователя
  "rating": 5000,        // Рейтинг
  "winPartiesCount": 150,  // Количество побед
  "partiesCount": 200,   // Всего матчей
  "rank": 1              // Позиция в таблице
}
```

Сортировка:
1. По рейтингу (по убыванию)
2. По количеству побед (по убыванию)

---

## 💾 Кэш

- **Ключ**: `leaderboard:top`
- **Время жизни**: 1 час
- **Хранилище**: Redis
- **Формат**: JSON массив

---

## 🐛 Если что-то не работает

### Ошибка: "Cannot connect to Redis"
```bash
# 1. Проверить, что Redis запущен
docker ps  # или redis-cli ping

# 2. Проверить строку подключения в appsettings.json
# 3. Проверить firewall / сетевые настройки
```

### Ошибка: "Connection refused"
```bash
# Убедиться, что Redis слушает на нужном порту
redis-cli -p 6379 ping
```

### Логирование
Включить debug логирование в `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "BilliardServer.Application.Leaderboard": "Debug"
    }
  }
}
```

---

## 📝 Примеры на разных языках

### JavaScript
```javascript
const leaderboard = await fetch('/api/leaderboard/top?limit=50')
  .then(r => r.json());
console.log(leaderboard);
```

### Python
```python
import requests
resp = requests.get('http://localhost:5000/api/leaderboard/top?limit=50')
print(resp.json())
```

### C#
```csharp
using var client = new HttpClient();
var response = await client.GetAsync(
  "http://localhost:5000/api/leaderboard/top?limit=50");
var json = await response.Content.ReadAsStringAsync();
```

---

## 📚 Полная документация

Для детальной информации смотрите:
- `IMPLEMENTATION_DETAILS.md` - Полная реализация
- `LEADERBOARD_README.md` - Документация сервиса
- `LEADERBOARD_EXAMPLES.md` - Примеры использования
- `REDIS_SETUP.md` - Установка Redis

---

## ✅ Чеклист

- [ ] Redis установлен и запущен
- [ ] Приложение запущено (`dotnet run`)
- [ ] Логи показывают успешную инициализацию
- [ ] API доступен через http://localhost:5000/api/leaderboard/top
- [ ] Возвращаются данные пользователей
- [ ] Redis кэш содержит данные (`redis-cli GET leaderboard:top`)

---

## 🎉 Готово!

Таблица лидеров полностью функциональна и кэшируется в Redis. 

При каждом запросе:
1. Сервис проверяет кэш в Redis
2. Если есть данные - возвращает их (быстро)
3. Если нет - обновляет из БД и кэширует на 1 час

Вы можете вручную обновить таблицу через endpoint `/api/leaderboard/refresh` (требует роль Admin).
