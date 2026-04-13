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
    }
}
