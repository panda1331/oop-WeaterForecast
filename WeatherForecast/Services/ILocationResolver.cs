using WeatherForecast.Models.Weather;

namespace WeatherForecast.Services
{
    public interface ILocationResolver
    {
        Coordinates ResolveCity(string city);
    }
}
