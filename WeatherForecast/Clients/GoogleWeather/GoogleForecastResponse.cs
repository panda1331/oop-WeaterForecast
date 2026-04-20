using System.Text.Json.Serialization;

namespace WeatherForecast.Clients.GoogleWeather
{
    public class GoogleForecastResponse
    {
        [JsonPropertyName("forecastDays")]
        public List<GoogleForecastDay> ForecastDays { get; set; } = new();
    }
}
