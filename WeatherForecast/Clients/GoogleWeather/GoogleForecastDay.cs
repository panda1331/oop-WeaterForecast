using System.Text.Json.Serialization;

namespace WeatherForecast.Clients.GoogleWeather
{
    public class GoogleForecastDay
    {
        [JsonPropertyName("displayDate")]
        public DisplayDate Date { get; set; }

        [JsonPropertyName("maxTemperature")]
        public TemperatureData MaxTemperature { get; set; }

        [JsonPropertyName("minTemperature")]
        public TemperatureData MinTemperature { get; set; }

        [JsonPropertyName("daytimeForecast")]
        public DaytimeForecast DayTime { get; set; }
    }

    public class DisplayDate
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }
        [JsonPropertyName("month")]
        public int Month { get; set; }
        [JsonPropertyName("day")]
        public int Day { get; set; }
    }

    public class TemperatureData
    {
        [JsonPropertyName("degrees")]
        public decimal Degrees { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }
    }

    public class DaytimeForecast
    {
        [JsonPropertyName("weatherForecast")]
        public WeatherCondition WeatherCondition { get; set; }

        [JsonPropertyName("relativeHumidity")]
        public int RelativeHumidity { get; set; }

        [JsonPropertyName("wind")]
        public Wind Wind { get; set; }
    }
    public class WeatherCondition
    {
        [JsonPropertyName("description")]
        public Description Description { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Description
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; }
    }

    public class Wind
    {
        [JsonPropertyName("speed")]
        public Speed Speed { get; set; }
    }

    public class Speed
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }
    }
}
