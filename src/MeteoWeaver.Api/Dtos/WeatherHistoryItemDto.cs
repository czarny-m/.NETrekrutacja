namespace MeteoWeaver.Api.Dtos;

public record WeatherHistoryItemDto(
    int SnapshotId,
    string CityName,
    string? CountryCode,
    DateTime ImportedAtUtc,
    double CurrentTemperature,
    double CurrentWindSpeed,
    string ComfortSummary);
