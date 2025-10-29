# ShoppingBasket

Basic instructions to build and run the server component.

## Prerequisites

- .NET 9 SDK installed and on PATH

## Run the server from the command line

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

## Run from Visual Studio

- Open the solution, set `ShoppingBasket.Server` as the startup project and press **F5** (debug) or **Ctrl+F5** (run without debugger). You can also edit `__launchSettings.json__` to change the `applicationUrl` for the project profile.

## Run tests

- From solution or test project folder:
  - `dotnet test`

## Notes

- If the server uses EF Core migrations at startup, ensure the target database exists and the connection string in `appsettings.json` (or environment variables) is correct.
- For front-end development, run the client separately (see project README or package.json in `shoppingbasket.client`) and configure a proxy (e.g. Vite) to the server URL above if needed

cd path/to/shoppingbasket.client
npm install
npm run dev

npx cypress open

# or headless:

npx cypress run
