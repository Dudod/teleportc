using System;

namespace Repository
{
    public class AirportEntity
    {   
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Latitude  { get; set; }
        public double Longitude  { get; set; }
        public double Elevation_Ft { get; set; } //Elevation in foots
        public string Iata_Code { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime? Deleted_At { get; set; }
    }
}