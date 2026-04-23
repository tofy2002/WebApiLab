# WebApiLab

ASP.NET Core 8 Web API project covering real-world patterns and security.

## Tech Stack
- **Framework:** ASP.NET Core 8 + Entity Framework Core
- **Database:** SQL Server
- **Cache:** Redis
- **Auth:** ASP.NET Core Identity + JWT
- **Extras:** AutoMapper, FluentValidation, Serilog, Swagger

## Features
- JWT authentication with refresh token rotation & revocation
- Role-based authorization via ASP.NET Core Identity
- Account lockout after 5 failed login attempts
- Rate limiting on login (5 requests/min)
- Redis distributed caching with automatic invalidation
- Pagination, search, and sorting on list endpoints
- Global exception handling with consistent error responses
- Security headers (CSP, X-Frame-Options, X-Content-Type-Options)
- Structured logging to console and rolling daily files

## Setup

1. Add your config to `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=WebApiLab;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JWT": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "WebApiLab",
    "Audience": "WebApiLabClient"
  }
}
```

2. Make sure Redis is running on `localhost:6379`

3. Apply migrations and run:
```bash
dotnet ef database update
dotnet run
```

Swagger UI: `https://localhost:{port}/swagger`

## Endpoints

| Resource | Endpoints |
|---|---|
| Auth | `POST /api/User/Register`, `Login`, `RefreshToken`, `RevokeToken` |
| Students | `GET/POST /api/Student`, `GET/PUT/DELETE /api/Student/{id}` |
| Departments | `GET/POST /api/Department`, `GET/PUT/DELETE /api/Department/{id}` |
