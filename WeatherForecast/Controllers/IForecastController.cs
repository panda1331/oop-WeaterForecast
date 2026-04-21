using WeatherForecast.Models.Weather;

namespace WeatherForecast.Controllers
{
    public interface IForecastController
    {
        Task<WeatherForecastModel> GetWeatherForecastAsync(decimal latitude, decimal longitude, int days, string provider = "openweather");
    }
}
