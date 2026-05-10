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

## API Endpoints

### Użytkownicy
- `POST /api/users` - rejestracja
- `POST /api/users/login` - logowanie
- `GET /api/users/{id}` - profil
- `PUT /api/users/{id}` - edycja (wymaga auth)
- `DELETE /api/users/{id}` - usunięcie (wymaga auth)

### Aukcje
- `GET /api/auctions` - lista (z paginacją i filtrowaniem)
- `GET /api/auctions/{id}` - szczegóły
- `POST /api/auctions` - tworzenie (wymaga auth)
- `PUT /api/auctions/{id}` - edycja (wymaga auth + owner)
- `DELETE /api/auctions/{id}` - usunięcie (wymaga auth + owner)

### Oferty
- `GET /api/auctions/{id}/bids` - historia ofert
- `POST /api/auctions/{id}/bids` - złożenie oferty (wymaga auth)

## Testy

```bash
dotnet test
```

## Architektura

```
src/
├── AuctionSystem.Api/           # Kontrolery, konfiguracja
├── AuctionSystem.Application/   # Serwisy, DTOs, walidacja
├── AuctionSystem.Core/          # Encje, interfejsy
└── AuctionSystem.Infrastructure/ # EF Core, repozytoria
```

## Technologie

- ASP.NET Core 10
- Entity Framework Core + PostgreSQL
- JWT Authentication
- React + Vite + Tailwind CSS
- xUnit + Moq
- Docker
