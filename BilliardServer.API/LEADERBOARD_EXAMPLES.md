# Примеры использования Leaderboard API

## Примеры запросов к API

### 1. Получить топ таблицу лидеров

#### Request
```http
GET /api/leaderboard/top?limit=10
Host: localhost:5000
```

#### Response (200 OK)
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
  {
    "userId": 2,
    "userName": "Player2",
    "rating": 4800,
    "winPartiesCount": 240,
    "partiesCount": 295,
    "rank": 2
  },
  {
    "userId": 3,
    "userName": "ProPlayer",
    "rating": 4600,
    "winPartiesCount": 230,
    "partiesCount": 290,
    "rank": 3
  }
]
```

### 2. Получить позицию конкретного пользователя

#### Request
```http
GET /api/leaderboard/rank/2
Host: localhost:5000
```

#### Response (200 OK)
```json
{
  "rank": 2
}
```

#### Response если пользователя нет (200 OK)
```json
{
  "rank": -1
}
```

### 3. Обновить таблицу лидеров (Admin only)

#### Request
```http
POST /api/leaderboard/refresh
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response (200 OK)
```json
{
  "message": "Leaderboard refreshed successfully"
}
```

#### Response если нет прав доступа (403 Forbidden)
```json
{
  "message": "User is not authorized"
}
```

---

## Примеры на разных языках

### JavaScript / TypeScript

```javascript
// Получить топ лидеров
async function getTopLeaderboard(limit = 100) {
  try {
    const response = await fetch(`/api/leaderboard/top?limit=${limit}`);
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    const leaderboard = await response.json();
    console.log('Топ лидеры:', leaderboard);
    return leaderboard;
  } catch (error) {
    console.error('Ошибка при получении таблицы лидеров:', error);
  }
}

// Получить рейтинг пользователя
async function getUserRank(userId) {
  try {
    const response = await fetch(`/api/leaderboard/rank/${userId}`);
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    const { rank } = await response.json();
    console.log(`Рейтинг пользователя ${userId}: ${rank}`);
    return rank;
  } catch (error) {
    console.error('Ошибка при получении рейтинга:', error);
  }
}

// Обновить таблицу лидеров (требует токен)
async function refreshLeaderboard(token) {
  try {
    const response = await fetch('/api/leaderboard/refresh', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    console.log(data.message);
    return true;
  } catch (error) {
    console.error('Ошибка при обновлении таблицы лидеров:', error);
    return false;
  }
}

// Использование
getTopLeaderboard(50);
getUserRank(1);
refreshLeaderboard('your-jwt-token');
```

### React Component

```jsx
import React, { useState, useEffect } from 'react';

function LeaderboardComponent() {
  const [leaderboard, setLeaderboard] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchLeaderboard();
  }, []);

  const fetchLeaderboard = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/leaderboard/top?limit=100');
      if (!response.ok) {
        throw new Error('Failed to fetch leaderboard');
      }
      const data = await response.json();
      setLeaderboard(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Загрузка...</div>;
  if (error) return <div>Ошибка: {error}</div>;

  return (
    <div className="leaderboard">
      <h1>Таблица лидеров</h1>
      <table>
        <thead>
          <tr>
            <th>Место</th>
            <th>Игрок</th>
            <th>Рейтинг</th>
            <th>Побед</th>
            <th>Матчей</th>
            <th>Процент побед</th>
          </tr>
        </thead>
        <tbody>
          {leaderboard.map((entry) => (
            <tr key={entry.userId}>
              <td>{entry.rank}</td>
              <td>{entry.userName}</td>
              <td>{entry.rating}</td>
              <td>{entry.winPartiesCount}</td>
              <td>{entry.partiesCount}</td>
              <td>
                {(
                  (entry.winPartiesCount / entry.partiesCount) * 100
                ).toFixed(1)}
                %
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default LeaderboardComponent;
```

### Python

```python
import requests
import json

BASE_URL = "http://localhost:5000"

def get_top_leaderboard(limit=100):
    """Получить топ таблицу лидеров"""
    try:
        response = requests.get(
            f"{BASE_URL}/api/leaderboard/top",
            params={"limit": limit}
        )
        response.raise_for_status()
        return response.json()
    except requests.exceptions.RequestException as e:
        print(f"Ошибка при получении таблицы лидеров: {e}")
        return []

def get_user_rank(user_id):
    """Получить рейтинг пользователя"""
    try:
        response = requests.get(
            f"{BASE_URL}/api/leaderboard/rank/{user_id}"
        )
        response.raise_for_status()
        data = response.json()
        return data.get("rank", -1)
    except requests.exceptions.RequestException as e:
        print(f"Ошибка при получении рейтинга: {e}")
        return -1

def refresh_leaderboard(token):
    """Обновить таблицу лидеров"""
    try:
        headers = {
            "Authorization": f"Bearer {token}",
            "Content-Type": "application/json"
        }
        response = requests.post(
            f"{BASE_URL}/api/leaderboard/refresh",
            headers=headers
        )
        response.raise_for_status()
        return response.json()
    except requests.exceptions.RequestException as e:
        print(f"Ошибка при обновлении таблицы лидеров: {e}")
        return None

# Использование
if __name__ == "__main__":
    # Получить топ 10 лидеров
    leaderboard = get_top_leaderboard(10)
    for entry in leaderboard:
        print(f"{entry['rank']}. {entry['userName']} - Рейтинг: {entry['rating']}")

    # Получить рейтинг игрока
    rank = get_user_rank(1)
    print(f"Игрок 1 имеет рейтинг: {rank}")
```

### cURL

```bash
# Получить топ 50 лидеров
curl -X GET "http://localhost:5000/api/leaderboard/top?limit=50" \
  -H "Content-Type: application/json"

# Получить рейтинг пользователя с ID 1
curl -X GET "http://localhost:5000/api/leaderboard/rank/1" \
  -H "Content-Type: application/json"

# Обновить таблицу лидеров (требует токен с ролью Admin)
curl -X POST "http://localhost:5000/api/leaderboard/refresh" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json"

# Сохранить результат в файл
curl -X GET "http://localhost:5000/api/leaderboard/top?limit=100" \
  -H "Content-Type: application/json" \
  -o leaderboard.json
```

### C# HttpClient

```csharp
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

public class LeaderboardClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5000";

    public LeaderboardClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<LeaderboardEntry>> GetTopLeaderboardAsync(int limit = 100)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{BaseUrl}/api/leaderboard/top?limit={limit}"
            );

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<LeaderboardEntry>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return new List<LeaderboardEntry>();
        }
    }

    public async Task<int> GetUserRankAsync(long userId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{BaseUrl}/api/leaderboard/rank/{userId}"
            );

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<JsonElement>(json);
            return data.GetProperty("rank").GetInt32();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return -1;
        }
    }

    public async Task<bool> RefreshLeaderboardAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, 
                $"{BaseUrl}/api/leaderboard/refresh");

            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return false;
        }
    }
}

public class LeaderboardEntry
{
    public long UserId { get; set; }
    public string UserName { get; set; }
    public int Rating { get; set; }
    public int WinPartiesCount { get; set; }
    public int PartiesCount { get; set; }
    public int Rank { get; set; }
}

// Использование
var client = new LeaderboardClient();
var leaderboard = await client.GetTopLeaderboardAsync(50);
var rank = await client.GetUserRankAsync(1);
```

---

## Тестирование производительности

### Параметры кэша

- **Время жизни кэша**: 1 час
- **Максимальный размер результата**: 1000 записей (при limit > 1000 ограничивается)
- **Хранилище**: Redis

### Рекомендации для оптимизации

1. Использовать `limit` параметр для ограничения размера ответа
2. Кэш автоматически обновляется каждый час
3. Вручную обновлять через `/api/leaderboard/refresh` после важных матчей
4. Мониторить Redis для проверки используемой памяти
