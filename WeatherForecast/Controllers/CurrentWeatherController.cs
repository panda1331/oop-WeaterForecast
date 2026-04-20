using WeatherForecast.Clients;
using WeatherForecast.Factories;
using WeatherForecast.Models.Weather;

namespace WeatherForecast.Controllers
{
    public class CurrentWeatherController : ICurrentWeatherController
    {
        private readonly IWeatherProviderFactory _factory;
        public CurrentWeatherController(IWeatherProviderFactory providerFactory)
        {
            _factory = providerFactory;
        }

        public async Task<CurrentWeather> GetCurrentWeatherAsync(decimal latitude, decimal longitude, string provider = "openweather")
        {
            var client = _factory.GetProvider(provider);
            var temperature = await client.LocationCurrentTemperature(latitude, longitude);
            return new CurrentWeather(temperature);
        } 
    }
}
