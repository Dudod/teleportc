using System.Threading.Tasks;

namespace Airports.Providers
{
    public interface IAirportsProvider
    {
        Task<Airport> GetAirportAsync(string iataCode, CancellationToken cancellationToken);
    }
}
