using WeatherForecast.Models.Weather;

namespace WeatherForecast.Controllers
{
    public interface IMultipleLocationController
    {
        Task<List<LocationTemperature>> GetMultipleTemperaturesAsync(List<Coordinates> locations, string provider = "openweather");
        Task<List<LocationTemperature>> GetMultipleTemperaturesByCitiesAsync(List<string> cities, string provider = "openweather");
    }
}
