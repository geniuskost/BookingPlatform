# BookingPlatform

Платформа для пошуку та бронювання готелів — командний проєкт на ASP.NET Core.

- [Технічне завдання](docs/TZ.md)

## Структура

- `src/BookingPlatform.Domain` — доменна модель та правила бронювання
- `src/BookingPlatform.Web` — вебзастосунок ASP.NET Core
- `tests/BookingPlatform.Domain.Tests` — модульні тести xUnit

## Запуск

```bash
dotnet build
dotnet test
dotnet run --project src/BookingPlatform.Web
```
