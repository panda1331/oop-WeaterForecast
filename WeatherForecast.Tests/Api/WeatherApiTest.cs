using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System;
using System.Collections.Generic;
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
                .ThrowsAsync(new ApiCallException("Forecast API is not available."));

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
    }
}
