using System.Net;

namespace WeatherForecast.Shared.Responses
{
    public record Success<T>(ushort Code, T Data, string Message);
    public static class Success
    {
        public static Success<T> Create<T>(HttpStatusCode code, string message, T data) => new((ushort)code, data, message);
        public static Success<T> Create<T>(ushort code, string message, T data) => new(code, data, message);
    }
}
