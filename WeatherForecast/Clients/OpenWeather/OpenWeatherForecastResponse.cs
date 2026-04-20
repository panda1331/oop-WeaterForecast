using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WeatherForecast.Clients.OpenWeather
{
    public class OpenWeatherForecastResponse
    {
        [JsonPropertyName("list")]
        public List<ForecastItem> List { get; set; } = new();
    }

    public class ForecastItem
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; } 

        [JsonPropertyName("main")]
        public ForecastMain Main { get; set; }

        [JsonPropertyName("weather")]
        public List<ForecastWeather> Weather { get; set; } = new();

        [JsonPropertyName("wind")]
        public ForecastWind Wind { get; set; }
    }

    public class ForecastMain
    {
        [JsonPropertyName("temp")]
        public decimal Temp { get; set; }

        [JsonPropertyName("temp_min")]
        public decimal TempMin { get; set; }

        [JsonPropertyName("temp_max")]
        public decimal TempMax { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

    public class ForecastWeather
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class ForecastWind
    {
        [JsonPropertyName("speed")]
        public decimal Speed { get; set; }
    }
}
