using System.Threading;
using System.Threading.Tasks;

namespace Airports.Providers
{
    public interface IExternalAirportProvider
    {
        Task<ExternalAirport> GetAirportAsync(string iataCode, CancellationToken cancellationToken);
    }
}
