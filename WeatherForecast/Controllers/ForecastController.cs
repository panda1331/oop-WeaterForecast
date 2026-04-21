using WeatherForecast.Factories;
using WeatherForecast.Models.Weather;

namespace WeatherForecast.Controllers
{
    public class ForecastController : IForecastController
    {
        private readonly IWeatherProviderFactory _factory;
        public ForecastController(IWeatherProviderFactory factory)
        {
            _factory = factory;
        }
        public async Task<WeatherForecastModel> GetWeatherForecastAsync(decimal latitude, decimal longitude, int days, string provider)
        {
            var client = _factory.GetForecastProvider(provider);
            return await client.GetForecastAsync(latitude, longitude, days);
        }
    }
}
