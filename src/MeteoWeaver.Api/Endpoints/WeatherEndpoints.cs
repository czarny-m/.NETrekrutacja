using MeteoWeaver.Api.Services;

namespace MeteoWeaver.Api.Endpoints;

public static class WeatherEndpoints
{
    public static IEndpointRouteBuilder MapWeatherEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/weather").WithTags("Weather");

        group.MapPost("/import/{city}", async (
            string city,
            WeatherImportService service,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return Results.BadRequest(new { message = "City is required." });
            }

            var result = await service.ImportAsync(city, cancellationToken);
            return result is null
                ? Results.NotFound(new { message = $"City '{city}' not found." })
                : Results.Ok(result);
        });

        group.MapGet("/{city}/summary", async (
            string city,
            WeatherQueryService service,
            CancellationToken cancellationToken) =>
        {
            var summary = await service.GetSummaryAsync(city, cancellationToken);
            return summary is null
                ? Results.NotFound(new { message = $"No data for city '{city}'." })
                : Results.Ok(summary);
        });

        group.MapGet("/{city}/history", async (
            string city,
            WeatherQueryService service,
            CancellationToken cancellationToken) =>
        {
            var history = await service.GetHistoryAsync(city, cancellationToken);
            return history.Count == 0
                ? Results.NotFound(new { message = $"No history for city '{city}'." })
                : Results.Ok(history);
        });

        group.MapDelete("/{city}", async (
            string city,
            WeatherQueryService service,
            CancellationToken cancellationToken) =>
        {
            var removed = await service.DeleteCityAsync(city, cancellationToken);
            return removed == 0
                ? Results.NotFound(new { message = $"No data to delete for city '{city}'." })
                : Results.Ok(new { message = $"Deleted snapshots for city '{city}'.", removed });
        });

        return app;
    }
}
