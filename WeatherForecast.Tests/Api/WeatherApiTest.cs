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
            var excpectedWeather = new CurrentWeather(22.5m);
            var mockController = new Mock<ICurrentWeatherController>();
            mockController
                .Setup(c => c.GetCurrentWeatherAsync(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ReturnsAsync(excpectedWeather);

            var result = await WeatherApi.HandleGetCurrentWeather(mockController.Object, latitude, longitude);

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
            var mockController = new Mock<ICurrentWeatherController>();
            var result = await WeatherApi.HandleGetCurrentWeather(mockController.Object, latitude, longitude);
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
            var mockController = new Mock<ICurrentWeatherController>();
            mockController
                .Setup(c => c.GetCurrentWeatherAsync(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ThrowsAsync(new ApiCallException("API is not available."));
            var result = await WeatherApi.HandleGetCurrentWeather(mockController.Object, latitude, longitude);
            var internalServerError = result.Result.Should().BeOfType<InternalServerError<Status>>().Subject;
            internalServerError.Value.Should().NotBeNull();
            internalServerError.Value!.Code.Should().Be(500);
            internalServerError.Value.Message.Should().Be("API is not available.");
        }


    }
}
