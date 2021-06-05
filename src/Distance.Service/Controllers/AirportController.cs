using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Airports.Providers;

namespace Distance.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AirportController : ControllerBase
    {
        private IAirportsProvider _airportsProvider;

        public AirportController(IAirportsProvider airportsProvider)
        {
            _airportsProvider = airportsProvider;
        }

        [HttpGet("{iataCode}")]
        public async Task<ActionResult<Airport>> GetAsync(string iataCode)
        {
            var airport = await _airportsProvider.GetAirportAsync(iataCode, HttpContext?.RequestAborted ?? CancellationToken.None);

            if(airport != default(Airport))
            {
                return Ok(airport);
            }

            return NotFound();
        }

        [HttpDelete("{iataCode}")]
        public async Task<IActionResult> DeleteAsync(string iataCode)
        {
            var deleted = await _airportsProvider.DeleteAirportAsync(iataCode);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }


    }
}
