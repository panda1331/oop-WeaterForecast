using WeatherForecast.Clients;
using WeatherForecast.Models.Weather;

namespace WeatherForecast.Controllers
{
    public class CurrentWeatherController(IWeatherDataClient client)
    {
        private readonly IWeatherDataClient _client = client;

        public async Task<CurrentWeather> GetCurrentWeather(decimal latitude, decimal longitude)
        {
            var temperature = await _client.LocationCurrentTemperature(latitude, longitude);
            return new(temperature);
        } 
    }
}
