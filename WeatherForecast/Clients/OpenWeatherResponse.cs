using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json.Serialization;

namespace WeatherForecast.Clients
{
    public class OpenWeatherResponse
    {
        [JsonPropertyName("main")]
        public required Nested Main { get; set; }
        public class Nested
        {
            [JsonPropertyName("temp")]
            public decimal Temp { get; set; }
        }
    }
}
