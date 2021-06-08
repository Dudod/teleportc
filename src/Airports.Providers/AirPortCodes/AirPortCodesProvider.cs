using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Airports.Providers.AirPortCodes
{
    public class AirPortCodesProvider : IExternalAirportProvider
    {
        private readonly HttpClient _client;

        public AirPortCodesProvider(HttpClient client)
        {
            _client = client;
        }

        public async Task<Airport> GetAirportAsync(string iataCode, CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string> { { "iata", iataCode } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            try
            {
                var response = await _client.PostAsync("single", encodedContent, cancellationToken);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<AirPortCodesResponse>(responseString);

                if (result.Status)
                {
                    return result.Airport.ToAirport();
                }
            }
            catch
            {
                // do nothing
            }
            
            return default;
        }
    }
}
