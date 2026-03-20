# Capstone Architecture (RentAPlace)

## 1) High-level layers

This application is a full-stack system split into three top-level parts:

- `frontend/` (Angular UI)
  - Standalone components (Home, Login/Register, Property Details, Owner Dashboard, Renter Dashboard)
  - Services wrap HTTP calls and attach JWT token (`src/app/services/*`)
  - Calls backend REST APIs and updates UI state.

- `backend/` (ASP.NET Core Web API)
  - Controllers expose REST endpoints (Authentication, Properties, Reservations, Messages)
  - EF Core + SQL Server persistence via `ApplicationDbContext`
  - JWT authentication + role-based authorization (`Owner` / `User`)
  - Swagger for API exploration.

- `testing/` (Automated tests)
  - xUnit tests referencing the backend project
  - Verifies important business rules (example: reservation overlap handling)

## 2) Main interaction flows

### Authentication flow
1. User/Owner opens Login/Register in the frontend.
2. Frontend calls backend auth endpoints.
3. Backend returns a JWT token.
4. Frontend stores the token in `localStorage`.
5. For protected APIs, Angular includes the token in requests.

### Property reservation flow
1. A user selects dates on a property details screen.
2. Frontend sends `POST /api/Reservation`.
3. Backend validates:
   - required dates
   - check-out after check-in
   - no overlapping reservations (excluding rejected ones)
4. Backend stores the reservation with status `Pending`.

### Messaging flow (Owner <-> Renter)
1. Owner and renter can open their dashboards.
2. Each dashboard fetches messages using `GET /api/Message`.
3. Messages are filtered for the currently selected reservation (property + the other user).
4. When sending a message, frontend calls `POST /api/Message`.
5. Backend sets `SenderId` from the logged-in JWT and saves the message to the `Messages` table.

## 3) Data model (EF Core)

The backend uses EF Core entities managed by `ApplicationDbContext`:

- `User`
  - Authentication identity, role (`Owner` or `User`)
  - Relationships to reservations and messages
- `Property`
  - Owner’s properties and images
- `Reservation`
  - Booking record linking property + renter + dates + status
- `Message`
  - Chat message linking sender + receiver + property + text + timestamp

## 4) Why the repo is organized this way

For capstone presentations and development:

- Each part can run independently in a separate terminal (`frontend`, `backend`, `testing`).
- Backend code is separated into controllers + data/model layers for readability.
- Tests are kept in a dedicated project so you can validate behavior quickly.

