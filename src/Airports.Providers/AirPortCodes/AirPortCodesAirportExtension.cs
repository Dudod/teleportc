namespace Airports.Providers.AirPortCodes
{
    public static class AirPortCodesAirportExtension
    {
        public static Airport ToAirport(this AirPortCodesAirport airport)
        {
            return airport == null ? default : new Airport
            {
                Name = airport.Name,
                Latitude = airport.Latitude,
                Longitude = airport.Longitude,
                ElevationFt = airport.Elevation,
                IataCode = airport.Iata
            };
        }
    }
}
