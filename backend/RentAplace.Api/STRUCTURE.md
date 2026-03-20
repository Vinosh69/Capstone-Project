# `RentAplace.Api` folder structure

This is the ASP.NET Core Web API project (`RentAplace.csproj`).

## Key folders

- `Controllers/`
  - REST endpoints grouped by feature (Auth, Property, Reservation, Message, etc.)

- `Data/`
  - EF Core `ApplicationDbContext` and persistence-related code.

- `Models/`
  - Entity classes (database models) and DTOs returned/accepted by the API.

- `Migrations/`
  - EF Core migration history (database schema changes).

- `Properties/`
  - `launchSettings.json` and local development configuration.

- `Views/`
  - Razor views (if used). You can ignore these during API-only usage.

- `wwwroot/`
  - Static files served by ASP.NET Core (images, bundled assets, etc.).

## Entry point

- `Program.cs`
  - Main application startup: DI registration, EF Core configuration, JWT auth, CORS, Swagger, and middleware pipeline.

