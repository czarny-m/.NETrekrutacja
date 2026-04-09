using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace MeteoWeaver.Api.External;

public class OpenMeteoClient
{
    private readonly HttpClient _httpClient;

    public OpenMeteoClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GeoResult?> SearchCityAsync(string city, CancellationToken cancellationToken = default)
    {
        var url =
            $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=en&format=json";

        var response = await _httpClient.GetFromJsonAsync<GeoSearchResponse>(url, cancellationToken);
        return response?.Results?.FirstOrDefault();
    }

    public async Task<ForecastResponse?> GetForecastAsync(
        double latitude,
        double longitude,
        string timezone,
        CancellationToken cancellationToken = default)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);

        var current = "temperature_2m,wind_speed_10m,weather_code,is_day";
        var hourly = "temperature_2m,apparent_temperature,precipitation,wind_speed_10m,weather_code";

        var url =
            $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&timezone={Uri.EscapeDataString(timezone)}&forecast_days=2&current={current}&hourly={hourly}";

        return await _httpClient.GetFromJsonAsync<ForecastResponse>(url, cancellationToken);
    }
}

public class GeoSearchResponse
{
    [JsonPropertyName("results")]
    public List<GeoResult>? Results { get; set; }
}

public class GeoResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = "auto";
}

public class ForecastResponse
{
    [JsonPropertyName("current")]
    public CurrentWeather? Current { get; set; }

    [JsonPropertyName("hourly")]
    public HourlyWeather? Hourly { get; set; }
}

public class CurrentWeather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature2m { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed10m { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }

    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }
}

public class HourlyWeather
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = new();

    [JsonPropertyName("temperature_2m")]
    public List<double> Temperature2m { get; set; } = new();

    [JsonPropertyName("apparent_temperature")]
    public List<double> ApparentTemperature { get; set; } = new();

    [JsonPropertyName("precipitation")]
    public List<double> Precipitation { get; set; } = new();

    [JsonPropertyName("wind_speed_10m")]
    public List<double> WindSpeed10m { get; set; } = new();

    [JsonPropertyName("weather_code")]
    public List<int> WeatherCode { get; set; } = new();
}
