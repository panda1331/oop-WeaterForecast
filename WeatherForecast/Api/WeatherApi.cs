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

        private static async Task<Results<Ok<Success<CurrentWeather>>, BadRequest<Status>, InternalServerError<Status>>> 
            HandleGetCurrentWeather([FromServices] CurrentWeatherController controller,
                                        [DefaultValue("18.300231990440125")] string lat,
                                        [DefaultValue("-64.8251590359234")] string lon)
        {
            try
            {
                var latitude = decimal.Parse(lat, CultureInfo.InvariantCulture);
                var longitude = decimal.Parse(lon, CultureInfo.InvariantCulture);

                var weather = await controller.GetCurrentWeather(latitude, longitude);

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
