using Airports.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using System.Threading.Tasks;

namespace Distance.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistanceController : ControllerBase
    {
        private readonly IDistanceService _distanceService;
        private readonly IAirportsProvider _airportsProvider;
        private readonly IMemoryCache _cache;

        public DistanceController(IMemoryCache cache, IDistanceService distanceService, IAirportsProvider airportsProvider)
        {
            _cache = cache;
            _distanceService = distanceService;
            _airportsProvider = airportsProvider;
        }

        /// <summary>
        /// Calculate distance between two Airports.
        /// </summary>
        /// <param name="from">IATA code of depature airport. Must be three letters.</param>
        /// <param name="to">IATA code of destination airport. Must be three letters.</param>
        /// <returns>Calculated distance in miles.</returns>
        [HttpGet("{from}/{to}")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<ActionResult<double>> GetDistanceAsync(string from, string to)
        {
            if(string.IsNullOrWhiteSpace(from))
            {
                return BadRequest($"{nameof(from)} is empty");
            }
            if (string.IsNullOrWhiteSpace(to))
            {
                return BadRequest($"{nameof(to)} is empty");
            }

            from = from.ToUpperInvariant();
            to = to.ToUpperInvariant();

            if (_cache.TryGetValue($"{from}#{to}", out double entity))
            {
                return entity;
            }

            var validator = new IataCodeValidator();

            if (!validator.Validate(from).IsValid)
            {
                return BadRequest($"{from}");
            }
            if (!validator.Validate(to).IsValid)
            {
                return BadRequest($"{to}");
            }

            var airportFrom = await _airportsProvider.GetAirportAsync(from, HttpContext?.RequestAborted ?? CancellationToken.None);

            if (airportFrom == default)
            {
                return NotFound($"{from}");
            }

            if (from  == to)
            {
                return 0;
            }

            var airportTo = await _airportsProvider.GetAirportAsync(to, HttpContext?.RequestAborted ?? CancellationToken.None);
            
            if (airportTo == default)
            {
                return NotFound($"{to}");
            }

            var result = _distanceService.CalulateDistance(airportFrom.Latitude, airportFrom.Longitude, airportTo.Latitude, airportTo.Longitude,
                HttpContext?.RequestAborted ?? CancellationToken.None);

            _cache.Set($"{from}#{to}", result);

            return result;
        }
    }
}
