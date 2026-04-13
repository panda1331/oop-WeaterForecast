using WeatherForecast.Models.Weather;

namespace WeatherForecast.Controllers
{
    public interface ICurrentWeatherController
    {
        Task<CurrentWeather> GetCurrentWeatherAsync(decimal latitude, decimal longitude);
    }
}
