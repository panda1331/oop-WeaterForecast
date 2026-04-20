namespace WeatherForecast.Clients
{
    public class GoogleWeatherDataClient : IWeatherDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoogleWeatherDataClient(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration.GetValue<string>("GOOGLE_WEATHER_BASE_URL") ?? "");
            _apiKey = configuration.GetValue<string>("GOOGLE_WEATHER_API_KEY") ?? "";
        }

        public Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
        {
            throw new NotImplementedException();
        }
    }
}
