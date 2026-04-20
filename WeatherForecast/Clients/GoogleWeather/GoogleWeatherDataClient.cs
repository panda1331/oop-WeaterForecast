namespace WeatherForecast.Clients.GoogleWeather
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

        public async Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
        {
            var response = await _httpClient.GetAsync($"currentConditions:lookup?key={_apiKey}&location.latitude={latitude}&location.longitude={longitude}");
            var data = await response.Content.ReadFromJsonAsync<GoogleWeatherResponse>();
            return data.Temperature.Degrees;
        }
    }
}
