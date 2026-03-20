# Backend tests

This is the test project for the ASP.NET Core backend in `../backend/`.

## Folders

- **Unit/**: no database, no network; tests for pure logic.
- **Integration/**: database/infrastructure involved.
- **E2E/**: full API surface tests (HTTP pipeline).

## Run

From `rentaplace-frontend/`:

```bash
dotnet test .\backend-tests\RentAplace.Tests.csproj
```

