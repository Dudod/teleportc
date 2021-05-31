using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Airports.Providers
{
    public class ExternalAirportProvider : IExternalAirportProvider
    {
        private readonly HttpClient _client;

        public ExternalAirportProvider(HttpClient client)
        {
            _client = client;
        }

        public async Task<ExternalAirport> GetAirportAsync(string iataCode, CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string> { { "iata", iataCode } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            try
            {
                var response = await _client.PostAsync("single", encodedContent, cancellationToken);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExternalAirport>(responseString);
            }
            catch
            {
                return default;
            }
        }
    }
}