using System.Text.Json;
using WeatherForecast.Models.Weather;
using WeatherForecast.Utils;

namespace WeatherForecast.Clients.GoogleWeather
{
    public class GoogleWeatherDataClient : IWeatherDataClient, IWeatherForecastClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public GoogleWeatherDataClient(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration.GetValue<string>("GOOGLE_WEATHER_BASE_URL") ?? "";
            _apiKey = configuration.GetValue<string>("GOOGLE_WEATHER_API_KEY") ?? "";
        }

        public async Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
        {
            try
            {
                var latStr = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var lonStr = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var url = $"{_baseUrl}currentConditions:lookup?key={_apiKey}&location.latitude={latStr}&location.longitude={lonStr}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new ApiCallException($"googleweather returned bad status: {(ushort)response.StatusCode}");

                var data = await response.Content.ReadFromJsonAsync<GoogleWeatherResponse>();
                return data?.Temperature?.Degrees ?? throw new ApiCallException("failed to decode response");
            }
            catch (JsonException)
            {
                throw new ApiCallException("failed to decode response");
            }
            catch (HttpRequestException e)
            {
                throw new ApiCallException($"failed to call googleweather: {e.Message}.", inner: e);
            }
        }

        public Task<WeatherForecastModel> GetForecastAsync(decimal latitude, decimal longitude, int days)
        {
            throw new NotImplementedException();
        }
    }
}
