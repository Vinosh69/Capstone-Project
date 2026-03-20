# Capstone Project (RentAPlace)

This repository is organized into three top-level folders so each part can be run independently:

- `frontend/` – Angular UI
- `backend/` – ASP.NET Core Web API (EF Core + JWT)
- `testing/` – automated tests (xUnit)

## Run locally (3 terminals)

### 1) Backend API
```powershell
cd backend/RentAplace.Api
dotnet run
```

### 2) Frontend (Angular)
```powershell
cd frontend
npm install
npm start -- --port 4201
```

### 3) Tests
```powershell
cd testing/backend-tests
dotnet test
```

## Architecture
See `ARCHITECTURE.md`.

