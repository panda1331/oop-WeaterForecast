using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Globalization;
using System.Text.Json;
using WeatherForecast.Models.Weather;
using WeatherForecast.Utils;

namespace WeatherForecast.Clients.OpenWeather
{
    public class OpenWeatherDataClient : IWeatherDataClient, IWeatherForecastClient
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
                var latStr = latitude.ToString(CultureInfo.InvariantCulture);
                var lonStr = longitude.ToString(CultureInfo.InvariantCulture);
                var url = $"weather?lat={latStr}&lon={lonStr}&appid={_apiKey}&units=metric";

                var response = await _client.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiCallException(
                        $"openweather returned bad status: {(ushort)response.StatusCode}"
                    );
                }

                var data = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>();
                return data?.Main?.Temp ?? throw new ApiCallException("failed to decode response");
            }
            catch (HttpRequestException e)
            {
                throw new ApiCallException($"failed to call openweather: {e.Message}.", inner: e);
            }
            catch(JsonException)
            {
                throw new ApiCallException("failed to decode response");
            }
        }

        public async Task<WeatherForecastModel> GetForecastAsync(decimal latitude, decimal longitude, int days)
        {
            try
            {
                var latStr = latitude.ToString(CultureInfo.InvariantCulture);
                var lonStr = longitude.ToString(CultureInfo.InvariantCulture);
                var daysStr = days.ToString(CultureInfo.InvariantCulture);

                var url = $"forecast?lat={latStr}&lon={lonStr}&appid={_apiKey}&units=metric";

                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new ApiCallException($"openweather returned bad status: {(ushort)response.StatusCode}");

                var data = await response.Content.ReadFromJsonAsync<OpenWeatherForecastResponse>();
                if (data?.List?.Any() != true)
                    throw new ApiCallException("failed to decode response");

                var dailyForecast = data.List
                    .GroupBy(item => DateTimeOffset.FromUnixTimeSeconds(item.Dt).Date)
                    .Select(day => new ForecastDay(
                        day.Key,
                        day.Min(i => i.Main.TempMin),
                        day.Max(i => i.Main.TempMax),
                        day.First().Weather.FirstOrDefault()?.Description ?? "unknown",
                        (int)day.Average(i => i.Main.Humidity),
                        day.Average(i => i.Wind.Speed))
                    )
                    .Take(days)
                    .ToList();

                return new WeatherForecastModel(dailyForecast);
            }
            catch (JsonException)
            {
                throw new ApiCallException("failed to decode response");
            }
            catch (HttpRequestException e)
            {
                throw new ApiCallException($"failed to call openweather: {e.Message}.", inner: e);
            }
        }
    }
}
