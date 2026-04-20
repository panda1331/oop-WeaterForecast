using WeatherForecast.Clients;
using WeatherForecast.Clients.GoogleWeather;
using WeatherForecast.Clients.OpenWeather;

namespace WeatherForecast.Factories
{
    public class WeatherProviderFactory : IWeatherProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public WeatherProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IWeatherDataClient GetProvider(string providerName)
        {
            return providerName.ToLower() switch
            {
                "openweather" => _serviceProvider.GetRequiredService<OpenWeatherDataClient>(),
                "google" => _serviceProvider.GetRequiredService<GoogleWeatherDataClient>(),
                _ => throw new ArgumentException($"Unknown provider {providerName}")
            };
        }
    }
}
