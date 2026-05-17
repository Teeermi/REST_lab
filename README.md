# System Aukcyjny

System aukcji internetowych zbudowany jako REST API z frontendem React.

## Wymagania

- .NET 10 SDK
- Node.js 22+
- PostgreSQL 16+ (lub Docker)

## Uruchomienie

### Z Docker (zalecane)

```bash
docker-compose up --build
```

Aplikacja będzie dostępna:
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Frontend: http://localhost:5173

### Ręcznie

1. Uruchom PostgreSQL i utwórz bazę `auctiondb`

2. Backend:
```bash
cd src/AuctionSystem.Api
dotnet ef database update --project ../AuctionSystem.Infrastructure
dotnet run
```

3. Frontend:
```bash
cd client
npm install
npm run dev
```

## Architektura

```
src/
├── AuctionSystem.Api/           # Kontrolery, middleware, konfiguracja
├── AuctionSystem.Application/   # Serwisy, DTOs, walidatory
├── AuctionSystem.Core/          # Encje domenowe, interfejsy repozytoriów
└── AuctionSystem.Infrastructure/ # EF Core, implementacje repozytoriów

tests/
└── AuctionSystem.Tests/         # Testy jednostkowe (xUnit + Moq)

client/                          # Frontend React + Vite + Tailwind CSS
```

## Diagram ERD

```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│     Users       │       │    Auctions     │       │      Bids       │
├─────────────────┤       ├─────────────────┤       ├─────────────────┤
│ Id (PK)         │       │ Id (PK)         │       │ Id (PK)         │
│ Email           │◄──────│ OwnerId (FK)    │       │ Amount          │
│ Username        │   1:N │ Title           │◄──────│ AuctionId (FK)  │
│ PasswordHash    │       │ Description     │   1:N │ BidderId (FK)   │──────►
│ CreatedAt       │       │ Category        │       │ CreatedAt       │
└─────────────────┘       │ StartingPrice   │       └─────────────────┘
        │                 │ CurrentPrice    │
        │                 │ StartDate       │
        │                 │ EndDate         │
        │     N:1         │ Status          │
        └─────────────────┴─────────────────┘
```

**Relacje:**
- User 1:N Auction (właściciel aukcji)
- User 1:N Bid (licytujący)
- Auction 1:N Bid (oferty w aukcji)

## API Endpoints

### Użytkownicy

| Metoda | Endpoint | Opis | Auth |
|--------|----------|------|------|
| POST | `/api/users` | Rejestracja nowego użytkownika | - |
| POST | `/api/users/login` | Logowanie (zwraca JWT) | - |
| GET | `/api/users/{id}` | Pobranie profilu użytkownika | - |
| PUT | `/api/users/{id}` | Aktualizacja profilu | JWT |
| DELETE | `/api/users/{id}` | Usunięcie konta | JWT |

### Aukcje

| Metoda | Endpoint | Opis | Auth |
|--------|----------|------|------|
| GET | `/api/auctions` | Lista aukcji (z paginacją, filtrowaniem, sortowaniem) | - |
| GET | `/api/auctions/{id}` | Szczegóły aukcji z historią ofert | - |
| POST | `/api/auctions` | Utworzenie nowej aukcji | JWT |
| PUT | `/api/auctions/{id}` | Edycja aukcji (tylko właściciel) | JWT |
| DELETE | `/api/auctions/{id}` | Usunięcie aukcji (tylko właściciel) | JWT |

### Oferty (Bids)

| Metoda | Endpoint | Opis | Auth |
|--------|----------|------|------|
| GET | `/api/auctions/{id}/bids` | Historia ofert aukcji | - |
| POST | `/api/auctions/{id}/bids` | Złożenie oferty | JWT |

### Parametry filtrowania (GET /api/auctions)

- `page` - numer strony (domyślnie 1)
- `pageSize` - liczba elementów na stronie (domyślnie 10)
- `category` - filtrowanie po kategorii (0-4)
- `status` - filtrowanie po statusie (0-3)
- `sortBy` - sortowanie: `price`, `price_desc`, `date`, `date_desc`

### Kody odpowiedzi HTTP

- `200 OK` - sukces (GET, PUT)
- `201 Created` - zasób utworzony (POST)
- `204 No Content` - sukces bez treści (DELETE)
- `400 Bad Request` - błąd walidacji
- `401 Unauthorized` - brak/nieprawidłowy token JWT
- `403 Forbidden` - brak uprawnień
- `404 Not Found` - zasób nie istnieje

## Kategorie

| ID | Nazwa |
|----|-------|
| 0 | Elektronika |
| 1 | Moda |
| 2 | Dom |
| 3 | Sport |
| 4 | Inne |

## Statusy aukcji

| ID | Nazwa |
|----|-------|
| 0 | Szkic |
| 1 | Aktywna |
| 2 | Zakończona |
| 3 | Anulowana |

## Technologie

**Backend:**
- ASP.NET Core 10
- Entity Framework Core + PostgreSQL
- JWT Authentication
- FluentValidation
- Serilog (logowanie)
- Swagger/OpenAPI

**Frontend:**
- React 19
- Vite
- Tailwind CSS
- Axios
- React Router

**Testy:**
- xUnit
- Moq

**DevOps:**
- Docker
- Docker Compose

## Testy

```bash
dotnet test
```

## Struktura projektu

```
REST_lab/
├── AuctionSystem.slnx
├── docker-compose.yml
├── Dockerfile
├── README.md
├── src/
│   ├── AuctionSystem.Api/
│   │   ├── Controllers/
│   │   │   ├── AuctionsController.cs
│   │   │   └── UsersController.cs
│   │   ├── Middleware/
│   │   │   └── ExceptionHandlingMiddleware.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── AuctionSystem.Application/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── Validators/
│   ├── AuctionSystem.Core/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Interfaces/
│   └── AuctionSystem.Infrastructure/
│       ├── Data/
│       ├── Migrations/
│       └── Repositories/
├── tests/
│   └── AuctionSystem.Tests/
└── client/
    └── src/
        ├── api/
        ├── components/
        ├── hooks/
        ├── pages/
        └── types/
```
