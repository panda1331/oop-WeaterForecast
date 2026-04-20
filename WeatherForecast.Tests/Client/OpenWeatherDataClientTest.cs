using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using WeatherForecast.Clients.OpenWeather;
using WeatherForecast.Utils;
using Xunit;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace WeatherForecast.Tests.Client
{
    public class OpenWeatherDataClientTest
    {
        private readonly IConfiguration _configuration;

        public OpenWeatherDataClientTest()
        {
            var settings = new Dictionary<string, string>
            {
                { "OPENWEATHER_BASE_URL", "https://api.test.com/" },
                { "OPENWEATHER_API_KEY", "test_api_key" }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        private OpenWeatherDataClient CreateClient(HttpStatusCode statusCode, string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent)
            };

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var httpClient = new HttpClient(handlerMock.Object);
            return new OpenWeatherDataClient(_configuration, httpClient);
        }
        private OpenWeatherDataClient CreateClientThatThrows(Exception exception)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(exception);
            var httpClient = new HttpClient(handlerMock.Object);
            return new OpenWeatherDataClient(_configuration, httpClient);
        }

        [Fact]
        public async Task LocationCurrentTemperature_WithValidResponse_ReturnTemperature()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;
            var expectedTemp = 22.5m;

            var response = "{\"main\": {\"temp\": 22.5}}";
            var client = CreateClient(HttpStatusCode.OK, response);
            
            var result = await client.LocationCurrentTemperature(latitude, longitude);

            result.Should().Be(expectedTemp);
        }

        [Fact]
        public async Task LocationCurrentTemperature_WithNotSuccessStatusCode_ThrowsApiCallException()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;

            var client = CreateClient(HttpStatusCode.InternalServerError, "");
            Func<Task> act = async () => await client.LocationCurrentTemperature(latitude, longitude);

            await act.Should()
                .ThrowAsync<ApiCallException>()
                .WithMessage("*bad status: 500*");
        }

        [Fact]
        public async Task LocationCurrentTemperature_WithInvalidJson_ThrowsApiCallException()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;

            var client = CreateClient(HttpStatusCode.OK, "{}");
            Func<Task> act = async () => await client.LocationCurrentTemperature(latitude, longitude);

            await act.Should()
                .ThrowAsync<ApiCallException>()
                .WithMessage("failed to decode response");
        }

        [Fact]
        public async Task LocationCurrentTemperature_WithNetworkError_ThrowsApiCallException()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;

            var client = CreateClientThatThrows(new HttpRequestException("NetworkTimeout"));
            Func<Task> act = async () => await client.LocationCurrentTemperature(latitude, longitude);

            await act.Should()
                .ThrowAsync<ApiCallException>()
                .WithMessage("*failed to call openweather*");
        }


        [Fact]
        public async Task GetForecastAsync_WithValidResponse_ReturnForecast()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;
            var days = 3;

            var jsonResponse = """
            {
                "list": [
                    {
                        "dt": 1745272800,
                        "main": { "temp": 15.2, "temp_min": 12.0, "temp_max": 18.5, "humidity": 65 },
                        "weather": [ { "description": "clear sky" } ],
                        "wind": { "speed": 4.5 }
                    },
                    {
                        "dt": 1745283600,
                        "main": { "temp": 14.1, "temp_min": 11.5, "temp_max": 17.8, "humidity": 70 },
                        "weather": [ { "description": "few clouds" } ],
                        "wind": { "speed": 5.1 }
                    }
                ]
            }
            """;
            var client = CreateClient(HttpStatusCode.OK, jsonResponse);
            var result = await client.GetForecastAsync(latitude, longitude, days);

            result.Should().NotBeNull();
            result.Days.Should().NotBeEmpty();
        }
    }
}
