using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airports.Providers;

namespace Distance.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AirportController : ControllerBase
    {
        private IAirportsProvider _airportsProvider;

        public AirportController(IAirportsProvider airportsProvider)
        {
            _airportsProvider = airportsProvider;
        }

        [HttpGet("{iataCode}")]
        public Task<Airport> GetAsync(string iataCode)
        {
            return _airportsProvider.GetAirportAsync(iataCode, HttpContext?.RequestAborted ?? CancellationToken.None);
        }
    }
}
