using System.Threading;
using System.Threading.Tasks;
using Repository;

namespace Airports.Providers
{
    public class AirportsProvider : IAirportsProvider
    {
        private readonly IRepository _repository;
        private readonly IExternalAirportProvider _externalProvider;

        public AirportsProvider (IRepository repository, IExternalAirportProvider externalProvider)
        {
            _repository = repository;
            _externalProvider = externalProvider;
        }

        public async Task<Airport> GetAirportAsync(string iataCode, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetAirportAsync(iataCode);

            if (entity != default(AirportEntity))
            {
                return new Airport(entity);
            }
            
            var externalAirport = await _externalProvider.GetAirportAsync(iataCode, cancellationToken);

            if (externalAirport != default(ExternalAirport))
            {
                var airport = new Airport(externalAirport);

                airport.Id = await _repository.AddAirportAsync(airport.ToEntity());

                return airport;
            }

            return default;
        }
    }
}
