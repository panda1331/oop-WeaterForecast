using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WeatherForecast.Clients;
using WeatherForecast.Controllers;
using WeatherForecast.Factories;
using WeatherForecast.Models.Weather;
using WeatherForecast.Services;
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

            var mockLocationResolver = new Mock<ILocationResolver>();
            
            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

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

            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

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

            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

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

            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

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

            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

            Func<Task> act = async () => await controller.GetCurrentWeatherAsync(latitude,longitude,provider);

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"Unknown provider {provider}");
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);
        }

        //---------------------GET CURRENT WEATHER BY CITY----------------//
        [Fact]
        public async Task GetCurrentWeatherByCityAsync_WithValidCity_ReturnsTemperature()
        {
            var city = "Minsk";
            var provider = "openweather";
            var coordinates = new Coordinates(53.8930m, 27.5674m);
            var expectedTemp = 18.5m;

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(city))
                .Returns(coordinates);

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(coordinates.Latitude, coordinates.Longitude))
                .ReturnsAsync(expectedTemp);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);

            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetCurrentWeatherByCityAsync(city, provider);
            result.Should().NotBeNull();
            result.Temperature.Should().Be(expectedTemp);
            mockLocationResolver.Verify(r => r.ResolveCity(city), Times.Once);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(coordinates.Latitude,coordinates.Longitude), Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeatherByCityAsync_WithDefaultProvider_UsesOpenWeather()
        {
            var city = "London";
            var coordinates = new Coordinates(51.5074m, -0.1278m);
            var expectedTemp = 15.2m;

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(city))
                .Returns(coordinates);

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(coordinates.Latitude, coordinates.Longitude))
                .ReturnsAsync(expectedTemp);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider("openweather"))
                .Returns(mockClient.Object);

            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetCurrentWeatherByCityAsync(city);
            result.Should().NotBeNull();
            result.Temperature.Should().Be(expectedTemp);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider("openweather"), Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeatherByCityAsync_WithGoogleProvider_CallsGoogleClient()
        {
            var city = "Tokyo";
            var provider = "google";
            var coordinates = new Coordinates(35.6762m, 139.6503m);
            var expectedTemp = 22.1m;

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(city))
                .Returns(coordinates);

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(coordinates.Latitude, coordinates.Longitude))
                .ReturnsAsync(expectedTemp);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);

            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetCurrentWeatherByCityAsync(city, provider);
            result.Should().NotBeNull();
            result.Temperature.Should().Be(expectedTemp);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(coordinates.Latitude, coordinates.Longitude), Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeatherByCityAsync_WithUnknownCity_ThrowsArgumentException()
        {
            var city = "Paris";
            var provider = "openweather";

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(city))
                .Throws(new ArgumentException($"Unknown city: {city}"));

            var mockFactory = new Mock<IWeatherProviderFactory>();
            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

            Func<Task> act = async () => await controller.GetCurrentWeatherByCityAsync(city, provider);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage($"*{city}*");
            mockLocationResolver.Verify(r => r.ResolveCity(city), Times.Once);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetCurrentWeatherByCityAsync_WithEmptyCity_ThrowsArgumentException()
        {
            var city = "";
            var provider = "openweather";

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(city))
                .Throws(new ArgumentException("City cannot be empty"));

            var mockFactory = new Mock<IWeatherProviderFactory>();
            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

            Func<Task> act = async () => await controller.GetCurrentWeatherByCityAsync(city, provider);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*cannot be empty*");
        }

        [Fact]
        public async Task GetCurrentWeatherByCityAsync_WhenClientThrows_PropagatesException()
        {
            var city = "Minsk";
            var provider = "openweather";
            var coordinates = new Coordinates(53.8930m, 27.5674m);

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(city))
                .Returns(coordinates);

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(coordinates.Latitude, coordinates.Longitude))
                .ThrowsAsync(new ApiCallException("API is not available"));

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);

            var controller = new CurrentWeatherController(mockFactory.Object, mockLocationResolver.Object);

            Func<Task> act = async () => await controller.GetCurrentWeatherByCityAsync(city, provider);

            await act.Should().ThrowAsync<ApiCallException>().WithMessage("API is not available");
        }
    }
}
