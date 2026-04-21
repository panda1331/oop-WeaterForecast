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

            groups
                .MapGet("forecast", WeatherApi.HandleGetWeatherForecast)
                .WithName("GetWeatherForecast")
                .WithDisplayName("Get Weather Forecast")
                .WithTags(["weather"])
                .WithDescription("Returns weather forecast for given coordinates");

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

        public static async Task<Results<Ok<Success<WeatherForecastModel>>, BadRequest<Status>, InternalServerError<Status>>>
            HandleGetWeatherForecast([FromServices] IForecastController controller,
                                    string? lat = null,
                                    string? lon = null,
                                    string? days = null,
                                    string? provider = null)
        {
            try
            {
                var latitude = decimal.Parse(lat ?? "18.300231990440125", CultureInfo.InvariantCulture);
                var longitude = decimal.Parse(lon ?? "-64.8251590359234", CultureInfo.InvariantCulture);
                var ds = int.Parse(days ?? "3", CultureInfo.InvariantCulture);
                var providerValue = provider ?? "openweather";

                var forecast = await controller.GetWeatherForecastAsync(latitude, longitude, ds, providerValue);
                return TypedResults.Ok(Success.Create(200, "success", forecast));
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
