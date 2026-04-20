using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Globalization;
using System.Net.NetworkInformation;
using WeatherForecast.Controllers;
using WeatherForecast.Models.Weather;
using WeatherForecast.Shared.Responses;
using WeatherForecast.Utils;

namespace WeatherForecast.Api
{
    public static class WeatherApi
    {
        public static RouteGroupBuilder MapCurrentWeatherApi(this RouteGroupBuilder groups)
        {
            groups
               .MapGet("weather", WeatherApi.HandleGetCurrentWeather)
               .WithName("GetCurrentWeather")
               .WithDisplayName("Get Current Weather")
               .WithTags(["weather"])
               .WithDescription("Returns current weather for given coordinates");

            return groups;
        }

        public static async Task<Results<Ok<Success<CurrentWeather>>, BadRequest<Status>, InternalServerError<Status>>> 
            HandleGetCurrentWeather([FromServices] ICurrentWeatherController controller,
                                        string? lat = null,
                                        string? lon = null,
                                        string? provider = null)
        {
            try
            {
                var latitude = decimal.Parse(lat ?? "18.300231990440125", CultureInfo.InvariantCulture);
                var longitude = decimal.Parse(lon ?? "-64.8251590359234", CultureInfo.InvariantCulture);
                var providerValue = provider ?? "openweather";

                var weather = await controller.GetCurrentWeatherAsync(latitude, longitude, providerValue);

                return TypedResults.Ok(Success.Create(200, "success", weather));
            }
            catch (FormatException)
            {
                return TypedResults.BadRequest(Status.Create(400, "invalid coordinates"));
            }
            catch (OverflowException)
            {
                return TypedResults.BadRequest(Status.Create(400, "invalid coordinates"));
            }
            catch (ApiCallException e)
            {
                return TypedResults.InternalServerError(Status.Create(500, e.Message));
            }
        }
    }
}
