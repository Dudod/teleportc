using System;
using Repository;

namespace Airports.Providers
{
    public class Airport
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Latitude  { get; set; }
        public double Longitude  { get; set; }
        public double ElevationFt { get; set; } //Elevation in foots
        public string IataCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeletedAt { get; set; }

        public Airport(AirportEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Latitude = entity.Latitude;
            Longitude = entity.Longitude;
            ElevationFt = entity.ElevationFt;
            IataCode = entity.IataCode;
            CreatedAt = entity.CreatedAt;
            DeletedAt = entity.DeletedAt;
        }

        public Airport(ExternalAirport airport)
        {
            Name = airport.Name;
            Latitude = airport.Latitude;
            Longitude = airport.Longitude;
            ElevationFt = airport.Elevation;
            IataCode = airport.IataCode;
        }

        public static AirportEntity ToEntity(this Airport airport)
        {
            return airport == default(Airport) ? default(AirportEntity) : new AirportEntity()
            {
                Id = airport.Id,
                Name = airport.Name,
                Latitude = airport.Latitude,
                Longitude = airport.Longitude,
                ElevationFt = airport.ElevationFt,
                IataCode = airport.IataCode,
                CreatedAt = airport.CreatedAt,
                DeletedAt = airport.DeletedAt
            };
        }
    }
}