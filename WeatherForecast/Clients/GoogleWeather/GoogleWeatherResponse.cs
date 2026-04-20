using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WeatherForecast.Clients.GoogleWeather
{
    public class GoogleWeatherResponse
    {
        [JsonPropertyName("temperature")]
        public required Nested Temperature { get; set; }
        public class Nested
        {
            [JsonPropertyName("degrees")]
            public decimal Degrees { get; set; }
        }
    }
}
