namespace MeteoWeaver.Api.Domain;

public class CityWeatherSnapshot
{
    public int Id { get; set; }
    public string CityName { get; set; } = default!;
    public string? CountryCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Timezone { get; set; } = default!;
    public DateTime ImportedAtUtc { get; set; }

    public double CurrentTemperature { get; set; }
    public double CurrentWindSpeed { get; set; }
    public int CurrentWeatherCode { get; set; }
    public int IsDay { get; set; }

    public string ComfortSummary { get; set; } = default!;

    public List<HourlyForecastEntry> HourlyForecastEntries { get; set; } = new();
}
