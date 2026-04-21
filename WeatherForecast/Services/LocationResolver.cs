using WeatherForecast.Models.Weather;

namespace WeatherForecast.Services
{
    public class LocationResolver : ILocationResolver
    {
        private readonly Dictionary<string, Coordinates> _cities;

        public LocationResolver()
        {
            _cities = new Dictionary<string, Coordinates>(StringComparer.OrdinalIgnoreCase)
            {
                ["minsk"] = new Coordinates(53.8930m, 27.5674m),
                ["london"] = new Coordinates(51.5074m, -0.1278m),
                ["tokyo"] = new Coordinates(35.6762m, 139.6503m),
                ["shanghai"] = new Coordinates(31.2304m, 121.4737m),
                ["warsaw"] = new Coordinates(52.2298m, 21.0122m)
            };
        }
        public Coordinates ResolveCity(string city)
        {
            throw new NotImplementedException();
        }
    }
}
