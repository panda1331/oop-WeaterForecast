
using WeatherForecast.Models.Weather;

namespace WeatherForecast.Clients
{
    public interface IWeatherForecastClient
    {
        Task<WeatherForecastModel> GetForecastAsync(decimal latitude, decimal longitude, int days);
    }
}
