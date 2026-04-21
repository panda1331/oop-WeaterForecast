using WeatherForecast.Clients;

namespace WeatherForecast.Factories
{
    public interface IWeatherProviderFactory
    {
        IWeatherDataClient GetCurrentWeatherProvider(string providerName);
        IWeatherForecastClient GetForecastProvider(string providerName);
    }
}
