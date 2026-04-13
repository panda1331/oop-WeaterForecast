using WeatherForecast.Clients;
using WeatherForecast.Models.Weather;

namespace WeatherForecast.Controllers
{
    public class CurrentWeatherController(IWeatherDataClient client) : ICurrentWeatherController
    {
        private readonly IWeatherDataClient _client = client;

        public async Task<CurrentWeather> GetCurrentWeatherAsync(decimal latitude, decimal longitude)
        {
            var temperature = await _client.LocationCurrentTemperature(latitude, longitude);
            return new(temperature);
        } 
    }
}
