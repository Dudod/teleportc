using System;

namespace Airports.Providers.AirPortCodes
{
    public class AirPortCodesAirport
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public string Iata { get; set; }
    }
}
