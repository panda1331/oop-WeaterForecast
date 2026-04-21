using WeatherForecast.Factories;
using WeatherForecast.Models.Weather;

namespace WeatherForecast.Controllers
{
    public class MultipleLocationController : IMultipleLocationController
    {
        private readonly IWeatherProviderFactory _factory;
        public MultipleLocationController(IWeatherProviderFactory factory)
        {
            _factory = factory;
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
    }
}
