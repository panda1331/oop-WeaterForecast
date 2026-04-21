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
        public Task<List<LocationTemperature>> GetMultipleTemperaturesAsync(List<Coordinates> locations, string provider = "openweather")
        {
            throw new NotImplementedException();
        }
    }
}
