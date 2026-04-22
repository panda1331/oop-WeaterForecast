using WeatherForecast.Factories;
using WeatherForecast.Models.Weather;
using WeatherForecast.Services;

namespace WeatherForecast.Controllers
{
    public class ForecastController : IForecastController
    {
        private readonly IWeatherProviderFactory _factory;
        private readonly ILocationResolver _locationResolver;
        public ForecastController(IWeatherProviderFactory factory, ILocationResolver locationResolver)
        {
            _factory = factory;
            _locationResolver = locationResolver;
        }
        public async Task<WeatherForecastModel> GetWeatherForecastAsync(decimal latitude, decimal longitude, int days, string provider = "openweather")
        {
            var client = _factory.GetForecastProvider(provider);
            return await client.GetForecastAsync(latitude, longitude, days);
        }

        public Task<WeatherForecastModel> GetWeatherForecastByCityAsync(string city, int days, string provider = "openweather")
        {
            throw new NotImplementedException();
        }
    }
}
