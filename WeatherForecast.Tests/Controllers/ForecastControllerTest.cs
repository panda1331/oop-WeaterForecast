using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WeatherForecast.Clients;
using WeatherForecast.Controllers;
using WeatherForecast.Factories;
using WeatherForecast.Models.Weather;
using WeatherForecast.Utils;
using Xunit;

namespace WeatherForecast.Tests.Controllers
{
    public class ForecastControllerTest
    {
        [Fact]
        public async Task GetWeatherForecastAsync_WithValidParams_ReturnsForecast()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var provider = "openweather";
            var days = 3;

            var expectedForecast = new WeatherForecastModel(new List<ForecastDay>());

            var mockClient = new Mock<IWeatherForecastClient>();
            mockClient
                .Setup(c => c.GetForecastAsync(latitude, longitude, days))
                .ReturnsAsync(expectedForecast);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetForecastProvider(provider))
                .Returns(mockClient.Object);

            var controller = new ForecastController(mockFactory.Object);
            var result = await controller.GetWeatherForecastAsync(latitude, longitude, days, provider);

            result.Should().NotBeNull();
            mockFactory.Verify(f => f.GetForecastProvider(provider), Times.Once);
            mockClient.Verify(c => c.GetForecastAsync(latitude, longitude, days), Times.Once);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_WithDefaultProvider_UsesOpenWeather()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var days = 3;

            var expectedForecast = new WeatherForecastModel(new List<ForecastDay>());

            var mockClient = new Mock<IWeatherForecastClient>();
            mockClient
                .Setup(c => c.GetForecastAsync(latitude, longitude, days))
                .ReturnsAsync(expectedForecast);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetForecastProvider("openweather"))
                .Returns(mockClient.Object);

            var controller = new ForecastController(mockFactory.Object);
            var result = await controller.GetWeatherForecastAsync(latitude, longitude, days);

            result.Should().NotBeNull();
            mockFactory.Verify(f => f.GetForecastProvider("openweather"), Times.Once);
            mockClient.Verify(c => c.GetForecastAsync(latitude, longitude, days), Times.Once);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_WithGoogleProvider_CallsGoogleClient()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var provider = "google";
            var days = 3;

            var expectedForecast = new WeatherForecastModel(new List<ForecastDay>());

            var mockClient = new Mock<IWeatherForecastClient>();
            mockClient
                .Setup(c => c.GetForecastAsync(latitude, longitude, days))
                .ReturnsAsync(expectedForecast);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetForecastProvider(provider))
                .Returns(mockClient.Object);

            var controller = new ForecastController(mockFactory.Object);
            var result = await controller.GetWeatherForecastAsync(latitude, longitude, days, provider);

            result.Should().NotBeNull();
            mockFactory.Verify(f => f.GetForecastProvider(provider), Times.Once);
            mockClient.Verify(c => c.GetForecastAsync(latitude, longitude, days), Times.Once);
        }

        [Fact]
        public async Task GetWeatherForecastAsync_WhenClientThrows_PropagatesException()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var days = 3;
            var provider = "openweather";

            var mockClient = new Mock<IWeatherForecastClient>();
            mockClient
                .Setup(c => c.GetForecastAsync(latitude, longitude, days))
                .ThrowsAsync(new ApiCallException("API is not available"));

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetForecastProvider(provider))
                .Returns(mockClient.Object);

            var controller = new ForecastController(mockFactory.Object);
            Func<Task> act = async () => await controller.GetWeatherForecastAsync(latitude, longitude, days, provider);

            await act.Should()
                .ThrowAsync<ApiCallException>()
                .WithMessage("API is not available");
        }

        [Fact]
        public async Task GetWeatherForecastAsync_WithUnknownProvider_ThrowsArgumentException()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var days = 3;
            var provider = "abc";

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetForecastProvider(provider))
                .Throws(new ArgumentException($"Unknown provider {provider}"));

            var controller = new ForecastController(mockFactory.Object);
            Func<Task> act = async () => await controller.GetWeatherForecastAsync(latitude, longitude, days, provider);

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"Unknown provider {provider}");
            mockFactory.Verify(f => f.GetForecastProvider(provider), Times.Once);
        }
    }
}
