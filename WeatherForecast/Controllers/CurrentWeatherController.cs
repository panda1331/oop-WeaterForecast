using WeatherForecast.Clients;
using WeatherForecast.Factories;
using WeatherForecast.Models.Weather;
using WeatherForecast.Services;

namespace WeatherForecast.Controllers
{
    public class CurrentWeatherController : ICurrentWeatherController
    {
        private readonly IWeatherProviderFactory _factory;
        private readonly ILocationResolver _locationResolver;
        public CurrentWeatherController(IWeatherProviderFactory providerFactory, ILocationResolver locationResolver)
        {
            _factory = providerFactory;
            _locationResolver = locationResolver;
        }

        public async Task<CurrentWeather> GetCurrentWeatherAsync(decimal latitude, decimal longitude, string provider = "openweather")
        {
            var client = _factory.GetCurrentWeatherProvider(provider);
            var temperature = await client.LocationCurrentTemperature(latitude, longitude);
            return new CurrentWeather(temperature);
        }

        public async Task<CurrentWeather> GetCurrentWeatherByCityAsync(string city, string provider = "openweather")
        {
            var coordinates = _locationResolver.ResolveCity(city);
            var client = _factory.GetCurrentWeatherProvider(provider);
            var temperature = await client.LocationCurrentTemperature(coordinates.Latitude, coordinates.Longitude);
            return new CurrentWeather(temperature);
        }
    }
}
