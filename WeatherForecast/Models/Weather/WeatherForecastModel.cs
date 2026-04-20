namespace WeatherForecast.Models.Weather
{
    public record WeatherForecastModel(string City, List<ForecastDay> Days) {}
}
