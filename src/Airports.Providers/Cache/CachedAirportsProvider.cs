using Microsoft.Extensions.Caching.Memory;
using Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Airports.Providers
{
    public class CachedAirportsProvider : AirportsProvider, IAirportsProvider
    {
        private readonly IMemoryCache _cache;

        public CachedAirportsProvider(IMemoryCache memoryCache, IRepository repo, IExternalAirportProvider externalAirportProvider) 
            : base(repo, externalAirportProvider)
        {
            _cache = memoryCache;
        }

        public new Task<Airport> GetAirportAsync(string iataCode, CancellationToken cancellationToken)
        {
            return _cache.GetOrCreateAsync(iataCode, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                return base.GetAirportAsync(iataCode, cancellationToken);
            });
        }

        public new Task AddAirportAsync(Airport airport)
        {
            if (!_cache.TryGetValue(airport.IataCode, out var _))
            {
                return base.AddAirportAsync(airport);
            }

            return Task.CompletedTask;
        }

        public new Task<bool> DeleteAirportAsync(string iataCode)
        {
            _cache.Remove(iataCode);

            return base.DeleteAirportAsync(iataCode);
        }
    }
}
