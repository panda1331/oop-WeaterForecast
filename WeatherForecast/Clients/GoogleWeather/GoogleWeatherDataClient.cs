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

        public async Task<WeatherForecastModel> GetForecastAsync(decimal latitude, decimal longitude, int days)
        {
            try
            {
                var latStr = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var lonStr = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var daysStr = days.ToString(System.Globalization.CultureInfo.InvariantCulture);

                var url = $"{_baseUrl}forecast/days:lookup?key={_apiKey}&location.latitude={latStr}&location.longitude={lonStr}&days={daysStr}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new ApiCallException($"googleweather returned bad status: {(ushort)response.StatusCode}");

                var data = await response.Content.ReadFromJsonAsync<GoogleForecastResponse>();
                if (data?.ForecastDays == null || data.ForecastDays.Count == 0)
                    throw new ApiCallException("failed to decode response");
                
                return new WeatherForecastModel
                (
                    data.ForecastDays.Select(d => new ForecastDay(
                        new DateTime(d.Date.Year, d.Date.Month, d.Date.Day),
                        d.MinTemperature?.Degrees ?? 0,
                        d.MaxTemperature?.Degrees ?? 0,
                        d.DayTime?.WeatherCondition?.Description?.Text ?? "unknown",
                        d.DayTime?.RelativeHumidity ?? 0,
                        d.DayTime?.Wind?.Speed?.Value ?? 0
                    )).ToList()
                );
            }
            catch (JsonException)
            {
                throw new ApiCallException("failed to decode response");
            }
            catch(HttpRequestException e)
            {
                throw new ApiCallException($"failed to call googleweather: {e.Message}.", inner: e);
            }

        }
    }
}
