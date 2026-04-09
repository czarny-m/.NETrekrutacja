using MeteoWeaver.Api.Domain;

namespace MeteoWeaver.Api.Services;

public class ComfortScoreCalculator
{
    public int Calculate(double temperature, double precipitation, double windSpeed, int weatherCode)
    {
        double score = 100;

        score -= Math.Abs(temperature - 21) * 3;
        score -= precipitation * 12;
        score -= windSpeed * 0.5;
        score -= GetWeatherPenalty(weatherCode);

        return Math.Clamp((int)Math.Round(score), 0, 100);
    }

    public string BuildSummary(IEnumerable<HourlyForecastEntry> entries)
    {
        var bestEntries = entries
            .OrderByDescending(x => x.ComfortScore)
            .ThenBy(x => x.Time)
            .Take(3)
            .ToList();

        if (!bestEntries.Any())
        {
            return "No hourly data available.";
        }

        return string.Join(" | ", bestEntries.Select(x =>
            $"{x.Time:yyyy-MM-dd HH:mm} -> score={x.ComfortScore}, temp={x.Temperature:0.#}°C, precipitation={x.Precipitation:0.#} mm, wind={x.WindSpeed:0.#} km/h"));
    }

    private static int GetWeatherPenalty(int code) => code switch
    {
        0 => 0,
        1 or 2 => 2,
        3 => 5,
        45 or 48 => 8,
        >= 51 and <= 67 => 15,
        >= 71 and <= 77 => 18,
        >= 80 and <= 82 => 20,
        >= 95 and <= 99 => 25,
        _ => 10
    };
}
