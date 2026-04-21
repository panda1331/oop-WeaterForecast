using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WeatherForecast.Clients;
using WeatherForecast.Controllers;
using WeatherForecast.Factories;
using WeatherForecast.Utils;
using Xunit;

namespace WeatherForecast.Tests.Controllers
{
    public class CurrentWeatherControllerTest
    {
        [Fact]
        public async Task GetCurrentWeatherAsync_WithValidParams_ReturnsCurrentWeather()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var provider = "openweather";
            var expectedTemp = 22.5m;

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(latitude, longitude))
                .ReturnsAsync(expectedTemp);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);
            
            var controller = new CurrentWeatherController(mockFactory.Object);

            var result = await controller.GetCurrentWeatherAsync(latitude, longitude, provider);

            result.Should().NotBeNull();
            result.Temperature.Should().Be(expectedTemp);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(latitude, longitude), Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_WithDefaultProvider_UsesOpenWeather()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var expectedTemp = 22.5m;

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(latitude, longitude))
                .ReturnsAsync(expectedTemp);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider("openweather"))
                .Returns(mockClient.Object);

            var controller = new CurrentWeatherController(mockFactory.Object);

            var result = await controller.GetCurrentWeatherAsync(latitude, longitude);

            result.Should().NotBeNull();
            result.Temperature.Should().Be(expectedTemp);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider("openweather"), Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_WithGoogleProvider_CallsGoogleClient()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var provider = "google";
            var expectedTemp = 22.5m;

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(latitude, longitude))
                .ReturnsAsync(expectedTemp);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);

            var controller = new CurrentWeatherController(mockFactory.Object);

            var result = await controller.GetCurrentWeatherAsync(latitude, longitude, provider);

            result.Should().NotBeNull();
            result.Temperature.Should().Be(expectedTemp);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(latitude, longitude), Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_WhenClientThrows_PropagatesException()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var provider = "openweather";

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ThrowsAsync(new ApiCallException("API is not available"));

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);

            var controller = new CurrentWeatherController(mockFactory.Object);

            Func<Task> act = async () => await controller.GetCurrentWeatherAsync(latitude,longitude, provider);

            await act.Should()
                .ThrowAsync<ApiCallException>()
                .WithMessage("API is not available");
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_WithUnknownProvider_ThrowsArgumentException()
        {
            var latitude = 53.8930m;
            var longitude = 27.5674m;
            var provider = "abc";
            
            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Throws(new ArgumentException($"Unknown provider {provider}"));

            var controller = new CurrentWeatherController (mockFactory.Object);

            Func<Task> act = async () => await controller.GetCurrentWeatherAsync(latitude,longitude,provider);

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"Unknown provider {provider}");
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);
        }
    }
}
