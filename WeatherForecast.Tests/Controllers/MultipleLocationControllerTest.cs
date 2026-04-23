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
        private readonly List<Coordinates> _testCoordinates = new()
        {
            new(53.8930m, 27.5674m), 
            new(51.5074m, -0.1278m),  
            new(35.6762m, 139.6503m)  
        };

        private readonly List<string> _testCities = new() { "Minsk", "London", "Tokyo" };
        private readonly List<decimal> _testTemperatures = new() { 18.5m, 15.2m, 22.1m };

        private Mock<IWeatherDataClient> CreateMockClient()
        {
            var mock = new Mock<IWeatherDataClient>();
            for (int i = 0; i < _testCoordinates.Count; i++)
            {
                mock.Setup(c => c.LocationCurrentTemperature(
                    _testCoordinates[i].Latitude,
                    _testCoordinates[i].Longitude))
                    .ReturnsAsync(_testTemperatures[i]);
            }
            return mock;
        }

        private Mock<IWeatherDataClient> CreateMockClientThatThrows()
        {
            var mock = new Mock<IWeatherDataClient>();
            mock.Setup(c => c.LocationCurrentTemperature(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .ThrowsAsync(new ApiCallException("API is not available"));
            return mock;
        }

        private Mock<ILocationResolver> CreateMockLocationResolver()
        {
            var mock = new Mock<ILocationResolver>();
            for (int i = 0; i < _testCities.Count; i++)
                mock.Setup(r => r.ResolveCity(_testCities[i])).Returns(_testCoordinates[i]);
            return mock;
        }

        private Mock<IWeatherProviderFactory> CreateMockFactory(string provider, IWeatherDataClient client)
        {
            var mock = new Mock<IWeatherProviderFactory>();
            mock.Setup(f => f.GetCurrentWeatherProvider(provider)).Returns(client);
            return mock;
        }


        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithValidLocations_ReturnsTemperatures()
        {
            var mockClient = CreateMockClient();
            var mockFactory = CreateMockFactory("openweather", mockClient.Object);
            var controller = new MultipleLocationController(mockFactory.Object, Mock.Of<ILocationResolver>());

            var result = await controller.GetMultipleTemperaturesAsync(_testCoordinates, "openweather");

            result.Should().HaveCount(3);
            for (int i = 0; i < 3; i++)
            {
                result[i].Latitude.Should().Be(_testCoordinates[i].Latitude);
                result[i].Longitude.Should().Be(_testCoordinates[i].Longitude);
                result[i].Temperature.Should().Be(_testTemperatures[i]);
            }
            mockFactory.Verify(f => f.GetCurrentWeatherProvider("openweather"), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithDefaultProvider_UsesOpenWeather()
        {
            var mockClient = CreateMockClient();
            var mockFactory = CreateMockFactory("openweather", mockClient.Object);
            var controller = new MultipleLocationController(mockFactory.Object, Mock.Of<ILocationResolver>());

            var result = await controller.GetMultipleTemperaturesAsync(_testCoordinates);

            result.Should().HaveCount(3);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider("openweather"), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithGoogleProvider_CallsGoogleClient()
        {
            var mockClient = CreateMockClient();
            var mockFactory = CreateMockFactory("google", mockClient.Object);
            var controller = new MultipleLocationController(mockFactory.Object, Mock.Of<ILocationResolver>());

            var result = await controller.GetMultipleTemperaturesAsync(_testCoordinates, "google");

            result.Should().HaveCount(3);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider("google"), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WhenClientThrows_PropagatesException()
        {
            var mockClient = CreateMockClientThatThrows();
            var mockFactory = CreateMockFactory("openweather", mockClient.Object);
            var controller = new MultipleLocationController(mockFactory.Object, Mock.Of<ILocationResolver>());

            Func<Task> act = async () => await controller.GetMultipleTemperaturesAsync(_testCoordinates);

            await act.Should().ThrowAsync<ApiCallException>().WithMessage("API is not available");
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithUnknownProvider_ThrowsArgumentException()
        {
            var mockFactory = new Mock<IWeatherProviderFactory>();
            mockFactory.Setup(f => f.GetCurrentWeatherProvider("abc")).Throws(new ArgumentException("Unknown provider abc"));
            var controller = new MultipleLocationController(mockFactory.Object, Mock.Of<ILocationResolver>());

            Func<Task> act = async () => await controller.GetMultipleTemperaturesAsync(_testCoordinates, "abc");

            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*abc*");
        }

        [Fact]
        public async Task GetMultipleTemperaturesAsync_WithEmptyList_ReturnsEmptyList()
        {
            var mockClient = new Mock<IWeatherDataClient>();
            var mockFactory = CreateMockFactory("openweather", mockClient.Object);
            var controller = new MultipleLocationController(mockFactory.Object, Mock.Of<ILocationResolver>());

            var result = await controller.GetMultipleTemperaturesAsync(new List<Coordinates>());

            result.Should().BeEmpty();
            mockClient.Verify(c => c.LocationCurrentTemperature(It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Never);
        }

        //--------------------- GetMultipleTemperaturesByCitiesAsync --------------------//
        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithValidCities_ReturnsTemperatures()
        {
            var mockClient = CreateMockClient();
            var mockFactory = CreateMockFactory("openweather", mockClient.Object);
            var mockLocationResolver = CreateMockLocationResolver();
            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetMultipleTemperaturesByCitiesAsync(_testCities, "openweather");

            result.Should().HaveCount(3);
            mockLocationResolver.Verify(r => r.ResolveCity(It.IsAny<string>()), Times.Exactly(3));
            mockFactory.Verify(f => f.GetCurrentWeatherProvider("openweather"), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithDefaultProvider_UsesOpenWeather()
        {
            var mockClient = CreateMockClient();
            var mockFactory = CreateMockFactory("openweather", mockClient.Object);
            var mockLocationResolver = CreateMockLocationResolver();
            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetMultipleTemperaturesByCitiesAsync(_testCities);

            result.Should().HaveCount(3);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider("openweather"), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithGoogleProvider_CallsGoogleClient()
        {
            var mockClient = CreateMockClient();
            var mockFactory = CreateMockFactory("google", mockClient.Object);
            var mockLocationResolver = CreateMockLocationResolver();
            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            var result = await controller.GetMultipleTemperaturesByCitiesAsync(_testCities, "google");

            result.Should().HaveCount(3);
            mockFactory.Verify(f => f.GetCurrentWeatherProvider("google"), Times.Once);
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithUnknownCity_ThrowsArgumentException()
        {
            var mockLocationResolver = new Mock<ILocationResolver>();
            mockLocationResolver.Setup(r => r.ResolveCity("Minsk")).Returns(_testCoordinates[0]);
            mockLocationResolver.Setup(r => r.ResolveCity("Paris")).Throws(new ArgumentException("Unknown city: Paris"));
            mockLocationResolver.Setup(r => r.ResolveCity("Tokyo")).Returns(_testCoordinates[2]);

            var mockFactory = new Mock<IWeatherProviderFactory>();
            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            var cities = new List<string> { "Minsk", "Paris", "Tokyo" };
            Func<Task> act = async () => await controller.GetMultipleTemperaturesByCitiesAsync(cities);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Paris*");
            mockFactory.Verify(f => f.GetCurrentWeatherProvider(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WithEmptyList_ReturnsEmptyList()
        {
            var controller = new MultipleLocationController(Mock.Of<IWeatherProviderFactory>(), Mock.Of<ILocationResolver>());

            var result = await controller.GetMultipleTemperaturesByCitiesAsync(new List<string>());

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetMultipleTemperaturesByCitiesAsync_WhenClientThrows_PropagatesException()
        {
            var mockClient = CreateMockClientThatThrows();
            var mockFactory = CreateMockFactory("openweather", mockClient.Object);
            var mockLocationResolver = CreateMockLocationResolver();
            var controller = new MultipleLocationController(mockFactory.Object, mockLocationResolver.Object);

            Func<Task> act = async () => await controller.GetMultipleTemperaturesByCitiesAsync(_testCities);

            await act.Should().ThrowAsync<ApiCallException>().WithMessage("API is not available");
        }
    }
}