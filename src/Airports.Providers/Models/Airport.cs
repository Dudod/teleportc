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
        public DateTime? DeletedAt { get; set; }

        public Airport() { }

        public Airport(AirportEntity entity)
        {
            if (entity != null)
            {
                Id = entity.Id;
                Name = entity.Name;
                Latitude = entity.Latitude;
                Longitude = entity.Longitude;
                ElevationFt = entity.Elevation_Ft;
                IataCode = entity.Iata_Code;
                CreatedAt = entity.Created_At;
                DeletedAt = entity.Deleted_At;
            }
        }

        public AirportEntity ToEntity()
        {
            return  new AirportEntity()
            {
                Id = Id,
                Name = Name,
                Latitude = Latitude,
                Longitude = Longitude,
                Elevation_Ft = ElevationFt,
                Iata_Code = IataCode,
                Created_At = CreatedAt,
                Deleted_At = DeletedAt
            };
        }
    }
}