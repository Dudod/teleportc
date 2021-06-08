using Airports.Providers;
using Distance.Service.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Distance.Service.Tests
{
    public class AirportsControllerTest
    {
        private readonly AirportsController _airportsController;

        public AirportsControllerTest()
        {
            var airportsProvider = new Mock<IAirportsProvider>();

            airportsProvider.Setup(x => x.GetAirportAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((string iataCode, CancellationToken cancellationToken) =>
                {
                    var airport = new Airport();
                    airport.IataCode = iataCode;
                    return Task.FromResult(airport);
                });
            airportsProvider.Setup(x => x.AddAirportAsync(It.IsAny<Airport>()))
                .Returns(() => Task.CompletedTask);
            airportsProvider.Setup(x => x.DeleteAirportAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(true));

            _airportsController = new AirportsController(airportsProvider.Object);
        }

        [Fact]
        public async Task GetAirportAsync_ForCorrectIataCod_ShouldReturnAirport()
        {
            // arrange
            var iataCode = "AAA";

            // act
            var responce = await _airportsController.GetAsync(iataCode);

            // assert
            var result = responce.Should().BeOfType<ActionResult<Airport>>().Which;
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Which;
            var airport = okResult.Value.Should().BeOfType<Airport>().Which;
            airport.IataCode.Should().Be(iataCode);
        }

        [Theory]
        [MemberData(nameof(WrongIataCodes))]
        public async Task GetAirportAsync_ForWrongIataCod_ShouldReturnBadRequest(string iataCode)
        {
            // act
            var responce = await _airportsController.GetAsync(iataCode);

            // assert
            var result = responce.Should().BeOfType<ActionResult<Airport>>().Which;
            var badResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Which;
            badResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task DeleteAirportAsync_ForCorrectIataCod_ShouldReturnTrue()
        {
            // arrange
            var iataCode = "AAA";

            // act
            var responce = await _airportsController.DeleteAsync(iataCode);

            // assert
            var result = responce.Should().BeOfType<NoContentResult>();
        }

        [Theory]
        [MemberData(nameof(WrongIataCodes))]
        public async Task DeleteAirportAsync_ForWrongIataCod_ShouldReturnBadRequest(string iataCode)
        {
            // act
            var responce = await _airportsController.DeleteAsync(iataCode);

            // assert
            var badResult = responce.Should().BeOfType<BadRequestObjectResult>().Which;
            badResult.StatusCode.Should().Be(400);
        }

        [Theory]
        [MemberData(nameof(WrongIataCodes))]
        public async Task AddAirportAsync_ForWrongIataCod_ShouldReturnBadRequest(string iataCode)
        {
            // arrange
            var airport = new Airport()
            {
                IataCode = iataCode
            };

            // act
            var responce = await _airportsController.AddAsync(airport);

            // assert
            var badResult = responce.Should().BeOfType<BadRequestObjectResult>().Which;
            badResult.StatusCode.Should().Be(400);
        }

        [Theory]
        [InlineData(-91.0)]
        [InlineData(91.0)]
        [InlineData(null)]
        public async Task AddAirportAsync_ForWrongLatitude_ShouldReturnBadRequest(double latitude)
        {
            // arrange
            var airport = new Airport()
            {
                Latitude = latitude
            };

            // act
            var responce = await _airportsController.AddAsync(airport);

            // assert
            var badResult = responce.Should().BeOfType<BadRequestObjectResult>().Which;
            badResult.StatusCode.Should().Be(400);
        }

        [Theory]
        [InlineData(-180.0)]
        [InlineData(180.0)]
        [InlineData(null)]
        public async Task AddAirportAsync_ForWrongLongitude_ShouldReturnBadRequest(double longitude)
        {
            // arrange
            var airport = new Airport()
            {
                Longitude = longitude
            };

            // act
            var responce = await _airportsController.AddAsync(airport);

            // assert
            var badResult = responce.Should().BeOfType<BadRequestObjectResult>().Which;
            badResult.StatusCode.Should().Be(400);
        }

        [Theory]
        [InlineData(-13699)]
        [InlineData(29030)]
        [InlineData(null)]
        public async Task AddAirportAsync_ForWrongElevationFt_ShouldReturnBadRequest(double elevationFt)
        {
            // arrange
            var airport = new Airport()
            {
                ElevationFt = elevationFt
            };

            // act
            var responce = await _airportsController.AddAsync(airport);

            // assert
            var badResult = responce.Should().BeOfType<BadRequestObjectResult>().Which;
            badResult.StatusCode.Should().Be(400);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task AddAirportAsync_ForWrongName_ShouldReturnBadRequest(string name)
        {
            // arrange
            var airport = new Airport()
            {
                Name = name
            };

            // act
            var responce = await _airportsController.AddAsync(airport);

            // assert
            var badResult = responce.Should().BeOfType<BadRequestObjectResult>().Which;
            badResult.StatusCode.Should().Be(400);
        }

        public static IEnumerable<object[]> WrongIataCodes =>
            new List<object[]>
            {
                new object[] { "" },
                new object[] { " " },
                new object[] { "   " },
                new object[] { null },
                new object[] { "123" },
                new object[] { "aa1" },
                new object[] { "aa" },
                new object[] { "aaaa" },
                new object[] { "a-a" },
                new object[] { "a=a" },
                new object[] { "a_a" },
                new object[] { "a+a" },
                new object[] { "a*a" },
                new object[] { "a/a" }
            };
    }
}
