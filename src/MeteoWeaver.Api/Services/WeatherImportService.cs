using MeteoWeaver.Api.Data;
using MeteoWeaver.Api.Domain;
using MeteoWeaver.Api.Dtos;
using MeteoWeaver.Api.External;

namespace MeteoWeaver.Api.Services;

public class WeatherImportService
{
    private readonly OpenMeteoClient _openMeteoClient;
    private readonly ComfortScoreCalculator _comfortScoreCalculator;
    private readonly AppDbContext _dbContext;

    public WeatherImportService(
        OpenMeteoClient openMeteoClient,
        ComfortScoreCalculator comfortScoreCalculator,
        AppDbContext dbContext)
    {
        _openMeteoClient = openMeteoClient;
        _comfortScoreCalculator = comfortScoreCalculator;
        _dbContext = dbContext;
    }

    public async Task<ImportWeatherResultDto?> ImportAsync(string city, CancellationToken cancellationToken = default)
    {
        var normalizedCity = city.Trim();

        var geo = await _openMeteoClient.SearchCityAsync(normalizedCity, cancellationToken);
        if (geo is null)
        {
            return null;
        }

        var forecast = await _openMeteoClient.GetForecastAsync(geo.Latitude, geo.Longitude, geo.Timezone, cancellationToken);
        if (forecast?.Current is null || forecast.Hourly is null)
        {
            throw new InvalidOperationException("Could not fetch forecast data from external API.");
        }

        var snapshot = new CityWeatherSnapshot
        {
            CityName = geo.Name,
            CountryCode = geo.CountryCode,
            Latitude = geo.Latitude,
            Longitude = geo.Longitude,
            Timezone = geo.Timezone,
            ImportedAtUtc = DateTime.UtcNow,
            CurrentTemperature = forecast.Current.Temperature2m,
            CurrentWindSpeed = forecast.Current.WindSpeed10m,
            CurrentWeatherCode = forecast.Current.WeatherCode,
            IsDay = forecast.Current.IsDay
        };

        var hourly = forecast.Hourly;
        var count = new[]
        {
            hourly.Time.Count,
            hourly.Temperature2m.Count,
            hourly.ApparentTemperature.Count,
            hourly.Precipitation.Count,
            hourly.WindSpeed10m.Count,
            hourly.WeatherCode.Count
        }.Min();

        for (var i = 0; i < Math.Min(count, 48); i++)
        {
            var entry = new HourlyForecastEntry
            {
                Time = DateTime.Parse(hourly.Time[i]),
                Temperature = hourly.Temperature2m[i],
                ApparentTemperature = hourly.ApparentTemperature[i],
                Precipitation = hourly.Precipitation[i],
                WindSpeed = hourly.WindSpeed10m[i],
                WeatherCode = hourly.WeatherCode[i],
                ComfortScore = _comfortScoreCalculator.Calculate(
                    hourly.Temperature2m[i],
                    hourly.Precipitation[i],
                    hourly.WindSpeed10m[i],
                    hourly.WeatherCode[i])
            };

            snapshot.HourlyForecastEntries.Add(entry);
        }

        snapshot.ComfortSummary = _comfortScoreCalculator.BuildSummary(snapshot.HourlyForecastEntries.Take(24));

        _dbContext.CityWeatherSnapshots.Add(snapshot);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ImportWeatherResultDto(
            snapshot.Id,
            snapshot.CityName,
            snapshot.CountryCode,
            snapshot.ImportedAtUtc,
            snapshot.CurrentTemperature,
            snapshot.CurrentWindSpeed,
            snapshot.ComfortSummary,
            snapshot.HourlyForecastEntries.Count);
    }
}
