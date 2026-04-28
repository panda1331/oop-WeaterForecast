using WeatherForecast.Clients;
using WeatherForecast.Clients.GoogleWeather;
using WeatherForecast.Clients.OpenWeather;

namespace WeatherForecast.Factories
{
    public class WeatherProviderFactory : IWeatherProviderFactory
    {
        private readonly Dictionary<string, IWeatherDataClient> _weatherDataClients;
        private readonly Dictionary<string, IWeatherForecastClient> _weatherForecastClients;
     
        public WeatherProviderFactory(OpenWeatherDataClient openWeatherDataClient, GoogleWeatherDataClient googleWeatherDataClient)
        {
            _weatherDataClients = new Dictionary<string, IWeatherDataClient>(StringComparer.OrdinalIgnoreCase)
            {
                ["openweather"] = openWeatherDataClient,
                ["google"] = googleWeatherDataClient,
            };
            _weatherForecastClients = new Dictionary<string, IWeatherForecastClient>(StringComparer.OrdinalIgnoreCase)
            {
                ["openweather"] = openWeatherDataClient,
                ["google"] = googleWeatherDataClient,
            };
        }

        public IWeatherDataClient GetCurrentWeatherProvider(string providerName)
        {
            if (_weatherDataClients.TryGetValue(providerName, out var client)) 
                return client;
            throw new ArgumentException($"Unknown provider {providerName}");
        }

        public IWeatherForecastClient GetForecastProvider(string providerName)
        {
            if (_weatherForecastClients.TryGetValue(providerName, out var client))
                return client;
            throw new ArgumentException($"Unknown provider {providerName}");
        }
    }
}
