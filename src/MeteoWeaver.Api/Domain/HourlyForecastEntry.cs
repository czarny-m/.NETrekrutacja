namespace MeteoWeaver.Api.Domain;

public class HourlyForecastEntry
{
    public int Id { get; set; }

    public int CityWeatherSnapshotId { get; set; }
    public CityWeatherSnapshot CityWeatherSnapshot { get; set; } = default!;

    public DateTime Time { get; set; }
    public double Temperature { get; set; }
    public double ApparentTemperature { get; set; }
    public double Precipitation { get; set; }
    public double WindSpeed { get; set; }
    public int WeatherCode { get; set; }
    public int ComfortScore { get; set; }
}
