namespace MeteoWeaver.Api.Dtos;

public record WeatherSummaryDto(
    string City,
    string? CountryCode,
    DateTime ImportedAtUtc,
    double CurrentTemperature,
    double CurrentWindSpeed,
    int CurrentWeatherCode,
    int IsDay,
    string ComfortSummary,
    object Trend,
    IEnumerable<HourlyComfortDto> BestHours);
