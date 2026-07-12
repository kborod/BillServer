# Настройка Redis для разработки

## Установка Redis

### Windows

#### Вариант 1: Docker (рекомендуется)
```powershell
# Установить Docker Desktop

# Запустить Redis контейнер
docker run -d -p 6379:6379 --name redis redis:latest

# Проверить статус
docker ps

# Остановить Redis
docker stop redis

# Запустить Redis снова
docker start redis
```

#### Вариант 2: Windows Subsystem for Linux (WSL)
```bash
# Обновить пакеты
sudo apt-get update

# Установить Redis
sudo apt-get install redis-server

# Запустить Redis
sudo service redis-server start

# Проверить статус
sudo service redis-server status
```

#### Вариант 3: Memurai (Redis для Windows)
1. Скачать с https://github.com/microsoftarchive/redis/releases
2. Установить
3. Redis запустится автоматически как служба Windows

### Linux / macOS

```bash
# macOS (Homebrew)
brew install redis
brew services start redis

# Linux (Ubuntu/Debian)
sudo apt-get install redis-server
sudo systemctl start redis-server
```

## Проверка подключения

### Через Redis CLI

```bash
# Подключиться к Redis
redis-cli

# Проверить подключение
ping
# Ожидаемый результат: PONG

# Выход
exit
```

### Через PowerShell

```powershell
# Установить Redis-CLI для Windows
# Или использовать Docker

docker exec redis redis-cli ping
# Ожидаемый результат: PONG
```

## Конфигурация в appsettings.json

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

## Проверка работы Leaderboard Service

### 1. Запустить приложение

```bash
dotnet run
```

### 2. Проверить логи

Ожидаемые логи при запуске:
```
[INF] Initializing leaderboard service...
[INF] Refreshing leaderboard...
[INF] Leaderboard refreshed successfully with X entries
[INF] Leaderboard initialized successfully
```

### 3. Протестировать API

```bash
# Получить топ 10 лидеров
curl -X GET "http://localhost:5000/api/leaderboard/top?limit=10"

# Получить рейтинг пользователя (ID 1)
curl -X GET "http://localhost:5000/api/leaderboard/rank/1"
```

## Мониторинг Redis

### Redis Commander (GUI для Redis)

```bash
# Установить (npm требуется)
npm install -g redis-commander

# Запустить
redis-commander

# Открыть браузер на http://localhost:8081
```

### Через redis-cli

```bash
# Подключиться
redis-cli

# Получить все ключи
KEYS *

# Получить значение ключа
GET leaderboard:top

# Получить информацию о памяти
INFO memory

# Очистить кэш (осторожно!)
FLUSHDB
```

## Отладка

### Если сервис не подключается к Redis

1. Проверить, что Redis запущен:
```bash
redis-cli ping
```

2. Проверить строку подключения в `appsettings.json`

3. Проверить логи приложения на ошибки подключения

4. Если используется Docker:
```bash
docker ps  # Проверить, запущен ли контейнер
docker logs redis  # Посмотреть логи контейнера
```

### Очистить кэш таблицы лидеров

```bash
redis-cli
DEL leaderboard:top
```

## Тестирование в продакшене

Для полноценного тестирования таблицы лидеров:

1. Убедиться, что у вас в БД есть пользователи с заполненными полями:
   - `Rating` - рейтинг пользователя
   - `WinPartiesCount` - количество побед
   - `PartiesCount` - всего матчей

2. Запустить сервис

3. Вызвать эндпоинт `/api/leaderboard/top` для проверки

4. Мониторить Redis для проверки кэширования
