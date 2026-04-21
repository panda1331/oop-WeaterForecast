using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;
using WeatherForecast.Api;
using WeatherForecast.Controllers;
using WeatherForecast.Models.Weather;
using WeatherForecast.Shared.Responses;
using WeatherForecast.Utils;
using Xunit;

namespace WeatherForecast.Tests.Api
{
    public class WeatherApiTest
    {
        //---------------------------HANDLE GET CURRENT WEATHER ---------------------------//
        [Fact]
        public async Task HandleGetCurrentWeather_WithValidCoordinates_ReturnOkResult()
        {
            var latitude = "53.8930";
            var longitude = "27.5674";
            var provider = "openweather";
            var expectedWeather = new CurrentWeather(22.5m);
            var mockController = new Mock<ICurrentWeatherController>();
            mockController
                .Setup(c => c.GetCurrentWeatherAsync(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ReturnsAsync(expectedWeather);

            var result = await WeatherApi.HandleGetCurrentWeather(mockController.Object, latitude, longitude, provider);

            var okResult = result.Result.Should().BeOfType<Ok<Success<CurrentWeather>>>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.Value!.Code.Should().Be(200);
            okResult.Value.Message.Should().Be("success");
            okResult.Value.Data.Temperature.Should().Be(22.5m);
        }

        [Fact]
        public async Task HandleGetCurrentWeather_WithNonNumericCoordinates_ReturnsBadRequest()
        {
            var latitude = "abc";
            var longitude = "14.2345";
            var provider = "openweather";
            var mockController = new Mock<ICurrentWeatherController>();
            var result = await WeatherApi.HandleGetCurrentWeather(mockController.Object, latitude, longitude, provider);
            var badRequestResult = result.Result.Should().BeOfType<BadRequest<Status>>().Subject;
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.Value!.Code.Should().Be(400);
            badRequestResult.Value.Message.Should().Be("invalid coordinates");
        }

        [Fact]
        public async Task HandleGetCurrentWeather_WithOverflowCoordinates_ReturnsBadRequest()
        {
            var latitude = "99999999999999999999999999999999999999";
            var longitude = "14.2345";
            var provider = "openweather";
            var mockController = new Mock<ICurrentWeatherController>();
            var result = await WeatherApi.HandleGetCurrentWeather(mockController.Object, latitude, longitude, provider);
            var badRequestResult = result.Result.Should().BeOfType<BadRequest<Status>>().Subject;
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.Value!.Code.Should().Be(400);
            badRequestResult.Value.Message.Should().Be("invalid coordinates");
        }

        [Fact]
        public async Task HandleGetCurrentWeather_WhenControllerThrowsApiException_ReturnsInternalServerError()
        {
            var latitude = "53.8930";
            var longitude = "27.5674";
            var provider = "openweather";
            var mockController = new Mock<ICurrentWeatherController>();
            mockController
                .Setup(c => c.GetCurrentWeatherAsync(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ThrowsAsync(new ApiCallException("API is not available."));
            var result = await WeatherApi.HandleGetCurrentWeather(mockController.Object, latitude, longitude, provider);
            var internalServerError = result.Result.Should().BeOfType<InternalServerError<Status>>().Subject;
            internalServerError.Value.Should().NotBeNull();
            internalServerError.Value!.Code.Should().Be(500);
            internalServerError.Value.Message.Should().Be("API is not available.");
        }

        //---------------------------HANDLE GET WEATHER FORECAST ----------------------------//
        [Fact]
        public async Task HandleGetWeatherForecast_WithValidParams_ReturnOkResult()
        {
            var latitude = "53.8930";
            var longitude = "27.5674";
            var days = "3";
            var provider = "openweather";
            var expectedForecast = new WeatherForecastModel(new List<ForecastDay>());

            var mockController = new Mock<IForecastController>();
            mockController
                .Setup(c => c.GetWeatherForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(expectedForecast);

            var result = await WeatherApi.HandleGetWeatherForecast(mockController.Object, latitude, longitude, days, provider);
            var okResult = result.Result.Should().BeOfType<Ok<Success<WeatherForecastModel>>>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.Value!.Code.Should().Be(200);
            okResult.Value.Message.Should().Be("success");
            okResult.Value.Data.Should().Be(expectedForecast);
        }

        [Fact]
        public async Task HandleGetWeatherForecast_WithNonNumericCoordinates_ReturnsBadRequest()
        {
            var latitude = "abc";
            var longitude = "14.2345";
            var days = "3";
            var provider = "openweather";
            var mockController = new Mock<IForecastController>();
            var result = await WeatherApi.HandleGetWeatherForecast(mockController.Object, latitude, longitude, days, provider);
            var badRequestResult = result.Result.Should().BeOfType<BadRequest<Status>>().Subject;
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.Value!.Code.Should().Be(400);
            badRequestResult.Value.Message.Should().Be("invalid coordinates");
        }

        [Fact]
        public async Task HandleGetWeatherForecast_WhenControllerThrowsApiException_ReturnsInternalServerError()
        {
            var latitude = "53.8930";
            var longitude = "27.5674";
            var days = "3";
            var provider = "openweather";
            var mockController = new Mock<IForecastController>();
            mockController
                .Setup(c => c.GetWeatherForecastAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new ApiCallException("API is not available."));

            var result = await WeatherApi.HandleGetWeatherForecast(mockController.Object, latitude, longitude, days, provider);
            var internalServerError = result.Result.Should().BeOfType<InternalServerError<Status>>().Subject;
            internalServerError.Value.Should().NotBeNull();
            internalServerError.Value!.Code.Should().Be(500);
            internalServerError.Value.Message.Should().Be("API is not available.");
        }

        [Fact]
        public async Task HandleGetWeatherForecast_WithDefaultParameters_ReturnsOkResult()
        {
            var mockController = new Mock<IForecastController>();
            mockController
                .Setup(c => c.GetWeatherForecastAsync(18.300231990440125m, -64.8251590359234m, 5, "openweather"))
                .ReturnsAsync(new WeatherForecastModel(new List<ForecastDay>()));

            var result = await WeatherApi.HandleGetWeatherForecast(mockController.Object, null, null, null, null);
            var okResult = result.Result.Should().BeOfType<Ok<Success<WeatherForecastModel>>>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.Value!.Code.Should().Be(200);
        }

        //---------------------------HANDLE GET MULTIPLE WEATHER ----------------//
        [Fact]
        public async Task HandleGetMultipleWeather_WithValidCoordinates_ReturnOkResult()
        {
            var lat1 = 53.8930m;
            var lon1 = 27.5674m;
            var ex1 = 18.5m;

            var lat2 = 51.5074m;
            var lon2 = -0.1278m;
            var ex2 = 15.2m;

            var lat3 = 35.6762m;
            var lon3 = 139.6503m;
            var ex3 = 22.1m;

            var locations = new List<Coordinates>()
            {
                new(lat1, lon1),
                new(lat2, lon2),
                new(lat3, lon3)
            };
            var provider = "openweather";
            var expectedResult = new List<LocationTemperature>()
            {
                new LocationTemperature(lat1, lon1, ex1),
                new LocationTemperature(lat2, lon2, ex2),
                new LocationTemperature(lat3, lon3, ex3),
            };
            var locStr = string.Create(CultureInfo.InvariantCulture, $"{lat1},{lon1};{lat2},{lon2};{lat3},{lon3}");

            var mockController = new Mock<IMultipleLocationController>();
            mockController
                .Setup(c => c.GetMultipleTemperaturesAsync(locations, provider))
                .ReturnsAsync(expectedResult);

            var result = await WeatherApi.HandleGetMultipleWeather(mockController.Object, locStr, provider);

            var okResult = result.Result.Should().BeOfType<Ok<Success<List<LocationTemperature>>>>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.Value!.Code.Should().Be(200);
            okResult.Value.Message.Should().Be("success");
            okResult.Value.Data.Should().HaveCount(3);
            okResult.Value.Data[0].Temperature.Should().Be(ex1);
            okResult.Value.Data[1].Temperature.Should().Be(ex2);
            okResult.Value.Data[2].Temperature.Should().Be(ex3);
        }

        [Fact]
        public async Task HandleGetMultipleWeather_WithInvalidFormat_ReturnsBadRequest()
        {
            var mockController = new Mock<IMultipleLocationController>();
            var result = await WeatherApi.HandleGetMultipleWeather(mockController.Object, "invalid", "openweather");

            var badRequest = result.Result.Should().BeOfType<BadRequest<Status>>().Subject;
            badRequest.Value!.Code.Should().Be(400);
            badRequest.Value.Message.Should().Be("invalid locations format");
        }

        [Fact]
        public async Task HandleGetMultipleWeather_WithNonNumericCoordinates_ReturnsBadRequest()
        {
            var mockController = new Mock<IMultipleLocationController>();
            var result = await WeatherApi.HandleGetMultipleWeather(mockController.Object, "abc,rwf;wer,uiol", "openweather");

            var badRequest = result.Result.Should().BeOfType<BadRequest<Status>>().Subject;
            badRequest.Value!.Code.Should().Be(400);
            badRequest.Value.Message.Should().Be("invalid coordinates");
        }

        [Fact]
        public async Task HandleGetMultipleWeather_WithDefaultProvider_UsesOpenWeather()
        {
            var lat1 = 53.8930m;
            var lon1 = 27.5674m;
            var ex1 = 18.5m;

            var lat2 = 51.5074m;
            var lon2 = -0.1278m;
            var ex2 = 15.2m;

            var lat3 = 35.6762m;
            var lon3 = 139.6503m;
            var ex3 = 22.1m;
            var locStr = string.Create(CultureInfo.InvariantCulture, $"{lat1},{lon1};{lat2},{lon2};{lat3},{lon3}");
            var expectedResult = new List<LocationTemperature> 
            {
                new(lat1, lon1, ex1),
                new(lat2 , lon2, ex2),
                new(lat3 , lon3, ex3),
            };

            var mockController = new Mock<IMultipleLocationController>();
            mockController
                .Setup(c => c.GetMultipleTemperaturesAsync(It.IsAny<List<Coordinates>>(), "openweather"))
                .ReturnsAsync(expectedResult);

            var result = await WeatherApi.HandleGetMultipleWeather(mockController.Object, locStr, null);
            var okResult = result.Result.Should().BeOfType<Ok<Success<List<LocationTemperature>>>>().Subject;
            okResult.Value!.Code.Should().Be(200);
        }

        [Fact]
        public async Task HandleGetMultipleWeather_WhenControllerThrows_ReturnsInternalServerError()
        {
            var mockController = new Mock<IMultipleLocationController>();
            mockController
                .Setup(c => c.GetMultipleTemperaturesAsync(It.IsAny<List<Coordinates>>(), It.IsAny<string>()))
                .ThrowsAsync(new ApiCallException("API is not available"));

            var result = await WeatherApi.HandleGetMultipleWeather(mockController.Object, "53.8930,27.5674", "google");
            var error = result.Result.Should().BeOfType<InternalServerError<Status>>().Subject;
            error.Value!.Code.Should().Be(500);
            error.Value.Message.Should().Be("API is not available");
        }
    }
}
