namespace WeatherForecast.Models.Weather
{
    public record ForecastDay (DateTime Date,
                                decimal MinTemperature,
                                decimal MaxTemperature,
                                string Condition,
                                int Humidity,
                                decimal WindSpeed) {}
}
