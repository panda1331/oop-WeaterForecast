using System.Net;

namespace WeatherForecast.Shared.Responses
{
    public record Status(ushort Code, string Message)
    {
        public static Status Create(HttpStatusCode code, string message) => new((ushort) code, message);
        public static Status Create(ushort code, string message) => new Status(code, message);
    }
}
