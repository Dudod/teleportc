using System;

namespace Airports.Providers
{
    public class ExternalAirport
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public string IataCode { get; set; }
    }
}
