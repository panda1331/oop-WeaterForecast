using WeatherForecast.Clients;

namespace WeatherForecast.Factories
{
    public interface IWeatherProviderFactory
    {
        IWeatherDataClient GetProvider(string providerName);
    }
}
