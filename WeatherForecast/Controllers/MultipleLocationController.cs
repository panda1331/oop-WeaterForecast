using WeatherForecast.Factories;
using WeatherForecast.Models.Weather;
using WeatherForecast.Services;

namespace WeatherForecast.Controllers
{
    public class MultipleLocationController : IMultipleLocationController
    {
        private readonly IWeatherProviderFactory _factory;
        private readonly ILocationResolver _locationResolver;
        public MultipleLocationController(IWeatherProviderFactory factory, ILocationResolver locationResolver)
        {
            _factory = factory;
            _locationResolver = locationResolver;
        }
        public async Task<List<LocationTemperature>> GetMultipleTemperaturesAsync(List<Coordinates> locations, string provider = "openweather")
        {
            var client = _factory.GetCurrentWeatherProvider(provider);
            var tasks = locations.Select(async loc =>
            {
                var temp = await client.LocationCurrentTemperature(loc.Latitude, loc.Longitude);
                return new LocationTemperature(loc.Latitude, loc.Longitude, temp);
            });

            var temperatures = await Task.WhenAll(tasks);
            return temperatures.ToList();
        }

        public Task<List<LocationTemperature>> GetMultipleTemperaturesByCitiesAsync(List<string> cities, string provider = "openweather")
        {
            throw new NotImplementedException();
        }
    }
}
