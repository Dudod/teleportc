using System;

namespace Repository
{
    public class AirportEntity
    {   public Guid Id { get; set; }
        public string Name { get; set; }
        public double Latitude  { get; set; }
        public double Longitude  { get; set; }
        public double ElevationFt { get; set; } //Elevation in foots
        public string IataCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}