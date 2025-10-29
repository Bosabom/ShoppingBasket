# ShoppingBasket

A small demo shopping-basket application with a .NET 9 Web API backend and a React (Vite) frontend. The project demonstrates item catalog, discounts, receipt creation and history.

## Project functionality

### Server (`ShoppingBasket.Server`)

- Exposes a minimal REST API for:
  - `GET /items` and `GET /items/{id}` — catalog of available items.
  - `GET /discounts` and `GET /discounts/{id}` — configured discounts.
  - `GET /receipts` and `GET /receipts/{id}` — receipts.
  - `GET /receipts/detailed/{id}` — receipt with ordered items expanded.
  - `GET /receipts/history` — short receipts list for history UI.
  - `POST /receipts` — create a receipt from a request with item quantities.
- Receipt creation:
  - Loads requested items, applies discounts and multi-item rules, calculates per-line and receipt totals, and persists the receipt.
  - Implements a multi‑buy example: "Buy 2 tins of soup and get a loaf of bread for half price" — the server computes eligible discounted bread units and applies the discount.
  - Supports percentage discounts (per-item) and multi-buy discounts (target item receives discount for eligible units).
- Persistence and mapping:
  - Uses Entity Framework Core with Npgsql (PostgreSQL) and applies EF migrations at startup.
  - Uses Mapster for DTO mapping to/from entities.
- Error handling:
  - Throws domain `BadRequestException` for invalid requests (e.g., all zero quantities, missing items) which the middleware maps to HTTP Problem responses.
- Tests:
  - Unit tests with `xUnit` exercise calculation logic and validation.
  - Integration / e2e tests may be added (Cypress used in the client project for UI tests).

### Client (`shoppingbasket.client`)

- Single page React app (Vite) that:
  - Renders a `BasketForm` where users enter quantities for Soup, Bread, Milk and Apples.
  - Shows available discounts inline (e.g. "Buy 2 tins of soup and get a loaf of bread for half price", "10% off apples").
  - Submits `POST /receipts` and displays the created receipt via `GeneratedReceipt` component.
  - Provides a `ReceiptsTable` page that fetches `GET /receipts/history` and lists past receipts.
- Form UX:
  - Uses `react-hook-form` for input handling and client-side validation.
  - Displays server error messages returned as problem details when receipt creation fails.
- Tests:
  - Cypress e2e tests (in `shoppingbasket.client/cypress`) for navigation, form behavior and history page rendering.
- HTTP:
  - Client uses a small `httpService` wrapper (axios or similar) to call the API. For local development use a Vite proxy or run client and server on the same origin.

## Tech stack

- Backend
  - .NET 9 / C#
  - ASP.NET Core Minimal API
  - Entity Framework Core (EF Core) with Npgsql (PostgreSQL provider)
  - Mapster (object-to-object mapping)
  - xUnit (unit tests)
- Frontend
  - React (function components)
  - Vite (dev server / build)
  - react-hook-form (form handling)
  - Cypress (end-to-end tests)
  - axios (via a small `httpService` wrapper) — used for API calls
- Dev / tooling
  - dotnet CLI (`dotnet restore`, `dotnet build`, `dotnet run`, `dotnet test`)
  - npm / Node.js (client, Vite, Cypress)
  - Git

## Quick start

1. Server

Basic instructions to build and run the server component.

### Prerequisites

- .NET 9 SDK installed

### Run the server from the command line

1. Open a terminal and change to the server project folder:
   - `cd path/to/ShoppingBasket.Server`
2. Restore packages:
   - `dotnet restore`
3. Build the project:
   - `dotnet build`
4. Run the project:
   - `dotnet run`

Optional: start the server on fixed URLs (useful for local proxying)

- Run HTTP only:
  - `dotnet run --urls "http://localhost:5204" --project ./ShoppingBasket.Server/ShoppingBasket.Server.csproj`
- Run both HTTP and HTTPS:
  - `dotnet run --urls "https://localhost:7051;http://localhost:5204" --project ./ShoppingBasket.Server/ShoppingBasket.Server.csproj`

### Run from Visual Studio

- Open the solution, set `ShoppingBasket.Server` as the startup project and press **F5** (debug) or **Ctrl+F5** (run without debugger). You can also edit `launchSettings.json` to change the `applicationUrl` for the project profile.

### Run unit tests

- From solution or test project folder:
  - `dotnet test`

2. Client

- For front-end development, run the client separately (if it wasn't run with server) (see project README or package.json in `shoppingbasket.client`) and configure a proxy (e.g. Vite) to the server URL above if needed

1. Open a terminal and change to the server project folder:
   - `cd path/to/shoppingbasket.client`
2. Run npm install:
   - `npm install`
3. Build the project:

   - `npm run dev`

4. Integration tests (e2e)

- From solution or test project folder:
  - `npx cypress open`
  # or headless:
  - `npx cypress run`

If you run the client separately, configure a Vite proxy to forward `/receipts`, `/items`, `/discounts` to the server URL.

3. Run tests
   - Server unit tests:
     ```bash
     dotnet test
     ```
   - Client e2e (Cypress):
     ```bash
     cd shoppingbasket.client
     npx cypress open
     # or headless:
     npx cypress run
     ```

## Notes and gotchas

- Ensure your `appsettings.json` (or environment variables) contains a valid PostgreSQL connection string for EF Core migrations or create the database manually prior to startup.
- If you run the client separately, either enable CORS in the server or configure the Vite proxy to avoid cross-origin issues.
- For IDE runs, adjust `ShoppingBasket.Server/Properties/launchSettings.json` `applicationUrl` or use the `--urls` flag when running with `dotnet run`.
