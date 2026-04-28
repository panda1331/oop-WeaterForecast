using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using WeatherForecast.Clients.GoogleWeather;
using WeatherForecast.Utils;
using Xunit;

namespace WeatherForecast.Tests.Client
{
    public class GoogleWeatherDataClientTest
    {
        private readonly IConfiguration _configuration;

        public GoogleWeatherDataClientTest()
        {
            var settings = new Dictionary<string, string>
            {
                { "GOOGLE_WEATHER_BASE_URL", "https://api.test.com/" },
                { "GOOGLE_WEATHER_API_KEY", "test_api_key" }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        private GoogleWeatherDataClient CreateClient(HttpStatusCode statusCode, string responseContent)
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
            return new GoogleWeatherDataClient(_configuration, httpClient);
        }
        private GoogleWeatherDataClient CreateClientThatThrows(Exception exception)
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
            return new GoogleWeatherDataClient(_configuration, httpClient);
        }

        [Fact]
        public async Task LocationCurrentTemperature_WithValidResponse_ReturnTemperature()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;
            var expectedTemp = 13.7m;

            var response = "{\"temperature\": {\"degrees\": 13.7}}";
            var client = CreateClient(HttpStatusCode.OK, response);

            var result = await client.LocationCurrentTemperature(latitude, longitude);

            result.Should()
                .Be(expectedTemp);
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
                .WithMessage("*failed to call googleweather*");
        }

        [Fact]
        public async Task GetForecastAsync_WithValidResponse_ReturnForecast()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;
            var days = 3;

            var jsonResponse = """
            {
                "forecastDays": [
                    {
                        "displayDate": { "year": 2026, "month": 4, "day": 22 },
                        "maxTemperature": { "degrees": 18.5, "unit": "CELSIUS" },
                        "minTemperature": { "degrees": 8.2, "unit": "CELSIUS" },
                        "daytimeForecast": {
                            "weatherCondition": {
                                "description": { "text": "Partly cloudy", "languageCode": "en" },
                                "type": "PARTLY_CLOUDY"
                            },
                            "relativeHumidity": 65,
                            "wind": {
                                "speed": { "value": 4.5, "unit": "KILOMETERS_PER_HOUR" }
                            }
                        }
                    },
                    {
                        "displayDate": { "year": 2026, "month": 4, "day": 23 },
                        "maxTemperature": { "degrees": 20.1, "unit": "CELSIUS" },
                        "minTemperature": { "degrees": 10.3, "unit": "CELSIUS" },
                        "daytimeForecast": {
                            "weatherCondition": {
                                "description": { "text": "Sunny", "languageCode": "en" },
                                "type": "CLEAR"
                            },
                            "relativeHumidity": 55,
                            "wind": {
                                "speed": { "value": 3.2, "unit": "KILOMETERS_PER_HOUR" }
                            }
                        }
                    }
                ]
            }
            """;

            var client = CreateClient(HttpStatusCode.OK, jsonResponse);

            var result = await client.GetForecastAsync(latitude, longitude, days);

            result.Should().NotBeNull();
            result.Days.Should().HaveCount(2);

            result.Days[0].Date.Should().Be(new DateTime(2026, 4, 22));
            result.Days[0].MaxTemperature.Should().Be(18.5m);
            result.Days[0].MinTemperature.Should().Be(8.2m);
            result.Days[0].Condition.Should().Be("Partly cloudy");
            result.Days[0].Humidity.Should().Be(65);
            result.Days[0].WindSpeed.Should().Be(4.5m);

            result.Days[1].Date.Should().Be(new DateTime(2026, 4, 23));
            result.Days[1].MaxTemperature.Should().Be(20.1m);
            result.Days[1].MinTemperature.Should().Be(10.3m);
            result.Days[1].Condition.Should().Be("Sunny");
            result.Days[1].Humidity.Should().Be(55);
            result.Days[1].WindSpeed.Should().Be(3.2m);
        }

        [Fact]
        public async Task GetForecastAsync_WithNonSuccessStatusCode_ThrowsApiCallException()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;
            var days = 3;

            var client = CreateClient(HttpStatusCode.InternalServerError, "");

            Func<Task> act = async () => await client.GetForecastAsync(latitude,longitude, days);

            await act.Should().ThrowAsync<ApiCallException>().WithMessage("*bad status: 500*");
        }

        [Fact]
        public async Task GetForecastAsync_WithInvalidJson_ThrowsApiCallException()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;
            var days = 3;

            var client = CreateClient(HttpStatusCode.OK, "{}");

            Func<Task> act = async () => await client.GetForecastAsync(latitude, longitude, days);

            await act.Should()
                .ThrowAsync<ApiCallException>()
                .WithMessage("failed to decode response");
        }

        [Fact]
        public async Task GetForecastAsync_WithEmptyForecastDays_ThrowsApiCallException()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;
            var days = 3;

            var jsonResponse = "{\"forecastDays\": []}";
            var client = CreateClient(HttpStatusCode.OK, jsonResponse);

            Func<Task> act = async () => await client.GetForecastAsync(latitude, longitude, days);

            await act.Should()
                .ThrowAsync<ApiCallException>()
                .WithMessage("failed to decode response");
        }

        [Fact]
        public async Task GetForecastAsync_WithNetworkError_ThrowsApiCallException()
        {
            var latitude = 55.7558m;
            var longitude = 37.6173m;
            var days = 3;

            var client = CreateClientThatThrows(new HttpRequestException("Network timeout"));
            Func<Task> act = async () => await client.GetForecastAsync(latitude, longitude, days);

            await act.Should()
                .ThrowAsync<ApiCallException>()
                .WithMessage("*failed to call googleweather*");
        }
    }
}
