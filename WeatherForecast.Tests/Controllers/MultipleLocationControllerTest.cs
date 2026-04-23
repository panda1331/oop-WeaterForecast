using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class MultipleLocationControllerTest
    {
        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithValidLocations_ReturnsTemperatures()
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

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(lat1, lon1))
                .ReturnsAsync(ex1);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat2, lon2))
                .ReturnsAsync(ex2);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat3, lon3))
                .ReturnsAsync(ex3);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);

            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetMultipleTemperaturesAsync(locations, provider);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            result[0].Latitude.Should().Be(lat1);
            result[0].Longitude.Should().Be(lon1);
            result[0].Temperature.Should().Be(ex1);

            result[1].Latitude.Should().Be(lat2);
            result[1].Longitude.Should().Be(lon2);
            result[1].Temperature.Should().Be(ex2);

            result[2].Latitude.Should().Be(lat3);
            result[2].Longitude.Should().Be(lon3);
            result[2].Temperature.Should().Be(ex3);

            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);

            mockClient.Verify(c => c.LocationCurrentTemperature(lat1, lon1), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat2, lon2), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat3, lon3), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithDefaultProvider_UsesOpenWeather()
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

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(lat1, lon1))
                .ReturnsAsync(ex1);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat2, lon2))
                .ReturnsAsync(ex2);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat3, lon3))
                .ReturnsAsync(ex3);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider("openweather"))
                .Returns(mockClient.Object);
            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetMultipleTemperaturesAsync(locations);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            result[0].Latitude.Should().Be(lat1);
            result[0].Longitude.Should().Be(lon1);
            result[0].Temperature.Should().Be(ex1);

            result[1].Latitude.Should().Be(lat2);
            result[1].Longitude.Should().Be(lon2);
            result[1].Temperature.Should().Be(ex2);

            result[2].Latitude.Should().Be(lat3);
            result[2].Longitude.Should().Be(lon3);
            result[2].Temperature.Should().Be(ex3);

            mockFactory.Verify(f => f.GetCurrentWeatherProvider("openweather"), Times.Once);

            mockClient.Verify(c => c.LocationCurrentTemperature(lat1, lon1), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat2, lon2), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat3, lon3), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithGoogleProvider_CallsGoogleClient()
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
            var provider = "google";

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(lat1, lon1))
                .ReturnsAsync(ex1);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat2, lon2))
                .ReturnsAsync(ex2);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat3, lon3))
                .ReturnsAsync(ex3);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);
            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetMultipleTemperaturesAsync(locations, provider);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            result[0].Latitude.Should().Be(lat1);
            result[0].Longitude.Should().Be(lon1);
            result[0].Temperature.Should().Be(ex1);

            result[1].Latitude.Should().Be(lat2);
            result[1].Longitude.Should().Be(lon2);
            result[1].Temperature.Should().Be(ex2);

            result[2].Latitude.Should().Be(lat3);
            result[2].Longitude.Should().Be(lon3);
            result[2].Temperature.Should().Be(ex3);

            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);

            mockClient.Verify(c => c.LocationCurrentTemperature(lat1, lon1), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat2, lon2), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat3, lon3), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WhenClientThrows_PropagatesException()
        {
            var lat1 = 53.8930m;
            var lon1 = 27.5674m;

            var lat2 = 51.5074m;
            var lon2 = -0.1278m;

            var lat3 = 35.6762m;
            var lon3 = 139.6503m;

            var locations = new List<Coordinates>()
            {
                new(lat1, lon1),
                new(lat2, lon2),
                new(lat3, lon3)
            };
            var provider = "openweather";

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(lat1, lon1))
                .ThrowsAsync(new ApiCallException("API is not available"));
            mockClient.Setup(c => c.LocationCurrentTemperature(lat2, lon2))
                .ThrowsAsync(new ApiCallException("API is not available"));
            mockClient.Setup(c => c.LocationCurrentTemperature(lat3, lon3))
                .ThrowsAsync(new ApiCallException("API is not available"));

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);
            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);
            Func<Task> act = async () => await controller.GetMultipleTemperaturesAsync(locations, provider);

            await act.Should()
                .ThrowAsync<ApiCallException>()
                .WithMessage("API is not available");
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithUnknownProvider_ThrowsArgumentException()
        {
            var lat1 = 53.8930m;
            var lon1 = 27.5674m;

            var lat2 = 51.5074m;
            var lon2 = -0.1278m;

            var lat3 = 35.6762m;
            var lon3 = 139.6503m;

            var locations = new List<Coordinates>()
            {
                new(lat1, lon1),
                new(lat2, lon2),
                new(lat3, lon3)
            };
            var provider = "abc";

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Throws(new ArgumentException($"Unknown provider {provider}"));
            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);
            Func<Task> act = async () => await controller.GetMultipleTemperaturesAsync(locations, provider);

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"Unknown provider {provider}");
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithEmptyList_ReturnsEmptyList()
        {
            var locations = new List<Coordinates>();
            var provider = "openweather";

            var mockClient = new Mock<IWeatherDataClient>();
            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);
            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetMultipleTemperaturesAsync(locations, provider);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
            mockClient.Verify(c => c.LocationCurrentTemperature(It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Never);
        }

        //--------------------- GetMultipleTemperaturesByCitiesAsync --------------------//
        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithValidCities_ReturnsTemperatures()
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

            var cities = new List<string>() { "Minsk", "London", "Tokyo" };
            var provider = "openweather";
            var coordinates = new List<Coordinates>()
            {
                new Coordinates(lat1, lon1),
                new Coordinates(lat2, lon2),
                new Coordinates (lat3, lon3)
            };

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[0]))
                .Returns(coordinates[0]);
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[1]))
                .Returns(coordinates[1]);
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[2]))
                .Returns(coordinates[2]);

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(lat1, lon1))
                .ReturnsAsync(ex1);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat2, lon2))
                .ReturnsAsync(ex2);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat3, lon3))
                .ReturnsAsync(ex3);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);
            var result = await controller.GetMultipleTemperaturesByCitiesAsync(cities, provider);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result[0].Temperature.Should().Be(18.5m);
            result[1].Temperature.Should().Be(15.2m);
            result[2].Temperature.Should().Be(22.1m);

            mockLocationResolver.Verify(r => r.ResolveCity(cities[0]), Times.Once);
            mockLocationResolver.Verify(r => r.ResolveCity(cities[1]), Times.Once);
            mockLocationResolver.Verify(r => r.ResolveCity(cities[2]), Times.Once);

            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);

            mockClient.Verify(c => c.LocationCurrentTemperature(lat1, lon1), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat2, lon2), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat3, lon3), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithDefaultProvider_UsesOpenWeather()
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

            var cities = new List<string>() { "Minsk", "London", "Tokyo" };
            var coordinates = new List<Coordinates>()
            {
                new Coordinates(lat1, lon1),
                new Coordinates(lat2, lon2),
                new Coordinates (lat3, lon3)
            };

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[0]))
                .Returns(coordinates[0]);
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[1]))
                .Returns(coordinates[1]);
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[2]))
                .Returns(coordinates[2]);

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(lat1, lon1))
                .ReturnsAsync(ex1);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat2, lon2))
                .ReturnsAsync(ex2);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat3, lon3))
                .ReturnsAsync(ex3);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider("openweather"))
                .Returns(mockClient.Object);

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);
            var result = await controller.GetMultipleTemperaturesByCitiesAsync(cities);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result[0].Temperature.Should().Be(18.5m);
            result[1].Temperature.Should().Be(15.2m);
            result[2].Temperature.Should().Be(22.1m);

            mockLocationResolver.Verify(r => r.ResolveCity(cities[0]), Times.Once);
            mockLocationResolver.Verify(r => r.ResolveCity(cities[1]), Times.Once);
            mockLocationResolver.Verify(r => r.ResolveCity(cities[2]), Times.Once);

            mockFactory.Verify(f => f.GetCurrentWeatherProvider("openweather"), Times.Once);

            mockClient.Verify(c => c.LocationCurrentTemperature(lat1, lon1), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat2, lon2), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat3, lon3), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithGoogleProvider_CallsGoogleClient()
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

            var cities = new List<string>() { "Minsk", "London", "Tokyo" };
            var provider = "google";
            var coordinates = new List<Coordinates>()
            {
                new Coordinates(lat1, lon1),
                new Coordinates(lat2, lon2),
                new Coordinates (lat3, lon3)
            };

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[0]))
                .Returns(coordinates[0]);
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[1]))
                .Returns(coordinates[1]);
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[2]))
                .Returns(coordinates[2]);

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(lat1, lon1))
                .ReturnsAsync(ex1);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat2, lon2))
                .ReturnsAsync(ex2);
            mockClient.Setup(c => c.LocationCurrentTemperature(lat3, lon3))
                .ReturnsAsync(ex3);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);
            var result = await controller.GetMultipleTemperaturesByCitiesAsync(cities, provider);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result[0].Temperature.Should().Be(18.5m);
            result[1].Temperature.Should().Be(15.2m);
            result[2].Temperature.Should().Be(22.1m);

            mockLocationResolver.Verify(r => r.ResolveCity(cities[0]), Times.Once);
            mockLocationResolver.Verify(r => r.ResolveCity(cities[1]), Times.Once);
            mockLocationResolver.Verify(r => r.ResolveCity(cities[2]), Times.Once);

            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);

            mockClient.Verify(c => c.LocationCurrentTemperature(lat1, lon1), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat2, lon2), Times.Once);
            mockClient.Verify(c => c.LocationCurrentTemperature(lat3, lon3), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithUnknownCity_ThrowsArgumentException()
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

            var cities = new List<string>() { "Minsk", "Paris", "Tokyo" };
            var provider = "openweather";
            var coordinates = new List<Coordinates>()
            {
                new Coordinates(lat1, lon1),
                new Coordinates(lat2, lon2),
                new Coordinates (lat3, lon3)
            };

            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[0]))
                .Returns(coordinates[0]);
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[1]))
                .Throws(new ArgumentException($"Unknown city: {cities[1]}"));
            mockLocationResolver
                .Setup(r => r.ResolveCity(cities[2]))
                .Returns(coordinates[2]);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            
            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);
            
            Func<Task> act = async () => await controller.GetMultipleTemperaturesByCitiesAsync(cities, provider);
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Paris*");
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(provider), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithEmptyList_ReturnsEmptyList()
        {
            var cities = new List<string>();
            var provider = "openweather";

            var mockClient = new Mock<IWeatherDataClient>();
            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);
            var mockLocationResolver = new Mock<ILocationResolver>();

            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetMultipleTemperaturesByCitiesAsync(cities, provider);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
            mockClient.Verify(c => c.LocationCurrentTemperature(It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Never);
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WhenClientThrows_PropagatesException()
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

            var cities = new List<string>() { "Minsk", "London", "Tokyo" };
            var provider = "openweather";
            var coordinates = new List<Coordinates>()
            {
                new Coordinates(lat1, lon1),
                new Coordinates(lat2, lon2),
                new Coordinates (lat3, lon3)
            };

            var mockLocationResolver = new Mock<ILocationResolver>();

            var mockClient = new Mock<IWeatherDataClient>();
            mockClient
                .Setup(c => c.LocationCurrentTemperature(lat1, lon1))
                .ThrowsAsync(new ApiCallException("API is not available"));
            mockClient.Setup(c => c.LocationCurrentTemperature(lat2, lon2))
                .ThrowsAsync(new ApiCallException("API is not available"));
            mockClient.Setup(c => c.LocationCurrentTemperature(lat3, lon3))
                .ThrowsAsync(new ApiCallException("API is not available"));

            var mockFactiry = new Mock<IWeatherProviderFactory>();
            mockFactiry
                .Setup(f => f.GetCurrentWeatherProvider(provider))
                .Returns(mockClient.Object);

            var controller = new MultipleLocationController(mockFactiry.Object, mockLocationResolver.Object);
            Func<Task> act = async () => await controller.GetMultipleTemperaturesByCitiesAsync(cities, provider);
            await act.Should().ThrowAsync<ApiCallException>().WithMessage("API is not available");
        }
    }
}