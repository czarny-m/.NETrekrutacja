namespace MeteoWeaver.Api.Dtos;

public record ImportWeatherResultDto(
    int SnapshotId,
    string CityName,
    string? CountryCode,
    DateTime ImportedAtUtc,
    double CurrentTemperature,
    double CurrentWindSpeed,
    string ComfortSummary,
    int SavedHours);
