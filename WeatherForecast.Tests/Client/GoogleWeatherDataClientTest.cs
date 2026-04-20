using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using WeatherForecast.Clients.GoogleWeather;
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

        [Fact]
        public async Task LocationCurrentTemperature_WithValidResponse_ReturnsTemperature()
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
    }
}
