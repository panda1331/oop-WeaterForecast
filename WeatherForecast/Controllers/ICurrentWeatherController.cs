using WeatherForecast.Factories;
using WeatherForecast.Models.Weather;

namespace WeatherForecast.Controllers
{
    public interface ICurrentWeatherController
    {
        Task<CurrentWeather> GetCurrentWeatherAsync(decimal latitude, decimal longitude, string provider = "openweather");
    }
}
