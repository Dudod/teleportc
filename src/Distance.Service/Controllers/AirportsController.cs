using Airports.Providers;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distance.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AirportsController : ControllerBase
    {
        private readonly IAirportsProvider _airportsProvider;

        public AirportsController(IAirportsProvider airportsProvider)
        {
            _airportsProvider = airportsProvider;
        }

        /// <summary>
        /// Get Airport from store or external provider.
        /// </summary>
        /// <param name="iataCode">IATA code of airports. Must be three letters.</param>
        /// <returns>Airport object.</returns>
        [HttpGet("{iataCode}")]
        [ProducesResponseType(typeof(Airport), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Airport>> GetAsync(string iataCode)
        {
            if (string.IsNullOrWhiteSpace(iataCode))
            {
                return BadRequest($"{nameof(iataCode)} is empty");
            }

            iataCode = iataCode.ToUpperInvariant();

            var validator = new IataCodeValidator();

            if (!validator.Validate(iataCode).IsValid)
            {
                return BadRequest($"{iataCode}");
            }

            var airport = await _airportsProvider.GetAirportAsync(iataCode, HttpContext?.RequestAborted ?? CancellationToken.None);

            if(airport != default(Airport))
            {
                return Ok(airport);
            }

            return NotFound();
        }

        /// <summary>
        /// Save Airport to store.
        /// </summary>
        /// <param name="airport">Airport model</param>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> AddAsync([FromBody]Airport airport)
        {
            if (!string.IsNullOrWhiteSpace(airport.IataCode))
            {
                airport.IataCode = airport.IataCode.ToUpperInvariant();
            }

            var validator = new AirportValidator();
            var result = validator.Validate(airport);

            if (!result.IsValid)
            {
                var sb = new StringBuilder();
                foreach (var e in result.Errors)
                {
                    sb.Append($"Property {e.PropertyName}: '{e.AttemptedValue}' - {e.ErrorMessage}");
                }

                return BadRequest(sb.ToString());
            }

            await _airportsProvider.AddAirportAsync(airport);

            return Ok();
        }

        /// <summary>
        /// Delete Airport from store.
        /// </summary>
        /// <param name="iataCode">IATA code of airports. Must be three letters.</param>
        [HttpDelete("{iataCode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAsync(string iataCode)
        {
            if (string.IsNullOrWhiteSpace(iataCode))
            {
                return BadRequest($"{nameof(iataCode)} is empty");
            }

            iataCode = iataCode.ToUpperInvariant();

            var validator = new IataCodeValidator();

            if (!validator.Validate(iataCode).IsValid)
            {
                return BadRequest($"{iataCode}");
            }

            var deleted = await _airportsProvider.DeleteAirportAsync(iataCode);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
