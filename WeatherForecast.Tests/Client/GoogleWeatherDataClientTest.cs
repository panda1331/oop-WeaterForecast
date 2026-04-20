using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
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
                .WithMessage("*failed to call openweather*");
        }
    }
}
