namespace MeteoWeaver.Api.Dtos;

public record HourlyComfortDto(
    DateTime Time,
    double Temperature,
    double Precipitation,
    double WindSpeed,
    int ComfortScore);
