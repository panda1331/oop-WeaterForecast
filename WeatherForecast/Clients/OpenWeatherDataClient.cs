using Microsoft.AspNetCore.DataProtection.KeyManagement;
using WeatherForecast.Utils;

namespace WeatherForecast.Clients
{
    public class OpenWeatherDataClient : IWeatherDataClient
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        public OpenWeatherDataClient(IConfiguration config, HttpClient httpClient)
        {
            _client = httpClient;
            _client.BaseAddress = new Uri(config.GetValue<string>("OPENWEATHER_BASE_URL") ?? "");
            _apiKey = config.GetValue<string>("OPENWEATHER_API_KEY") ?? "";
        }
        public async Task<decimal> LocationCurrentTemperature(decimal latitude, decimal longitude)
        {
            try
            {
                var response = await _client.GetAsync(
                    $"weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric");

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiCallException(
                        $"openweather returned bad status: {(ushort)response.StatusCode}"
                    );
                }

                var data = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>();
                return data?.Main?.Temp ?? throw new ApiCallException($"failed to decode response");
            }
            catch (HttpRequestException e)
            {
                throw new ApiCallException($"failed to call openweather: {e.Message}.", inner: e);
            }
        }
    }
}
