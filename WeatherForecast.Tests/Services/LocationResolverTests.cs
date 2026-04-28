using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using WeatherForecast.Services;
using Xunit;

namespace WeatherForecast.Tests.Services
{
    public class LocationResolverTests
    {
        [Fact]
        public void ResolveCity_WithValidCity_ReturnsCoordinates()
        {
            var resolver = new LocationResolver();
            var result = resolver.ResolveCity("Minsk");

            result.Latitude.Should().Be(53.8930m);
            result.Longitude.Should().Be(27.5674m);
        }

        [Fact]
        public void ResolveCity_WithDifferentCase_ReturnsCoordinates()
        {
            var resolver = new LocationResolver();
            var result = resolver.ResolveCity("LONDON");

            result.Latitude.Should().Be(51.5074m);
            result.Longitude.Should().Be(-0.1278m);
        }

        [Fact]
        public void ResolveCity_WithSpaces_ReturnsCoordinates()
        {
            var resolver = new LocationResolver();
            var result = resolver.ResolveCity("  Tokyo  ");

            result.Latitude.Should().Be(35.68952m);
            result.Longitude.Should().Be(139.69171m);
        }

        [Fact]
        public void ResolveCity_WithUnknownCity_ThrowsArgumentException()
        {
            var resolver = new LocationResolver();
            Action act = () => resolver.ResolveCity("Paris");

            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("*Unknown city*");
        }

        [Fact]
        public void ResolveCity_WithEmptyString_ThrowsArgumentException()
        {
            var resolver = new LocationResolver();
            Action act = () => resolver.ResolveCity("");

            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("*cannot be empty*");
        }
    }
}
