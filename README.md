# MeteoWeaver

MeteoWeaver is a .NET 8 Minimal API application that:
- imports weather data from Open-Meteo,
- stores snapshots in SQLite,
- calculates a custom Comfort Score,
- exposes processed data through HTTP endpoints.

## Tech stack
- .NET 8
- ASP.NET Core Minimal API
- EF Core + SQLite
- Swagger / OpenAPI
- xUnit

## Requirements
- .NET 8 SDK

Check installation:
```bash
dotnet --list-sdks
```

## Run locally

From the repository root:
```bash
dotnet restore
dotnet run --project .\src\MeteoWeaver.Api\MeteoWeaver.Api.csproj
```

## Test
```bash
dotnet test
```

## Swagger
After application startup, open:
- `https://localhost:xxxx/swagger`
- or the HTTP url shown in terminal

## Endpoints

### Import city weather
`POST /api/weather/import/{city}`

Example:
`POST /api/weather/import/Warsaw`

### Get latest summary
`GET /api/weather/{city}/summary`

### Get city history
`GET /api/weather/{city}/history`

### Delete city data
`DELETE /api/weather/{city}`

## Custom functionality
The application calculates a custom Comfort Score for hourly weather data based on:
- temperature,
- precipitation,
- wind speed,
- weather code.

Then it returns the best upcoming hours for outdoor activity and allows comparing the latest snapshot with the previous one.
