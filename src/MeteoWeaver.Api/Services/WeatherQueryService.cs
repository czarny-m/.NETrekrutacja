using MeteoWeaver.Api.Data;
using MeteoWeaver.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MeteoWeaver.Api.Services;

public class WeatherQueryService
{
    private readonly AppDbContext _dbContext;

    public WeatherQueryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WeatherSummaryDto?> GetSummaryAsync(string city, CancellationToken cancellationToken = default)
    {
        var normalizedCity = city.Trim().ToLowerInvariant();

        var latest = await _dbContext.CityWeatherSnapshots
            .AsNoTracking()
            .Include(x => x.HourlyForecastEntries)
            .Where(x => x.CityName.ToLower() == normalizedCity)
            .OrderByDescending(x => x.ImportedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is null)
        {
            return null;
        }

        var previous = await _dbContext.CityWeatherSnapshots
            .AsNoTracking()
            .Where(x => x.CityName.ToLower() == normalizedCity)
            .OrderByDescending(x => x.ImportedAtUtc)
            .Skip(1)
            .FirstOrDefaultAsync(cancellationToken);

        var trend = previous is null
            ? new { message = "No previous snapshot." }
            : new
            {
                temperatureDelta = Math.Round(latest.CurrentTemperature - previous.CurrentTemperature, 1),
                windDelta = Math.Round(latest.CurrentWindSpeed - previous.CurrentWindSpeed, 1)
            };

        var bestHours = latest.HourlyForecastEntries
            .OrderByDescending(x => x.ComfortScore)
            .ThenBy(x => x.Time)
            .Take(3)
            .Select(x => new HourlyComfortDto(
                x.Time,
                x.Temperature,
                x.Precipitation,
                x.WindSpeed,
                x.ComfortScore))
            .ToList();

        return new WeatherSummaryDto(
            latest.CityName,
            latest.CountryCode,
            latest.ImportedAtUtc,
            latest.CurrentTemperature,
            latest.CurrentWindSpeed,
            latest.CurrentWeatherCode,
            latest.IsDay,
            latest.ComfortSummary,
            trend,
            bestHours);
    }

    public async Task<List<WeatherHistoryItemDto>> GetHistoryAsync(string city, CancellationToken cancellationToken = default)
    {
        var normalizedCity = city.Trim().ToLowerInvariant();

        return await _dbContext.CityWeatherSnapshots
            .AsNoTracking()
            .Where(x => x.CityName.ToLower() == normalizedCity)
            .OrderByDescending(x => x.ImportedAtUtc)
            .Select(x => new WeatherHistoryItemDto(
                x.Id,
                x.CityName,
                x.CountryCode,
                x.ImportedAtUtc,
                x.CurrentTemperature,
                x.CurrentWindSpeed,
                x.ComfortSummary))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteCityAsync(string city, CancellationToken cancellationToken = default)
    {
        var normalizedCity = city.Trim().ToLowerInvariant();

        var items = await _dbContext.CityWeatherSnapshots
            .Where(x => x.CityName.ToLower() == normalizedCity)
            .ToListAsync(cancellationToken);

        if (items.Count == 0)
        {
            return 0;
        }

        _dbContext.CityWeatherSnapshots.RemoveRange(items);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return items.Count;
    }
}
