using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace Repository
{
    public class Repository : IRepository
    {
        private string _connectionString;

        Repository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public Task<AirportEntity> GetAirportAsync(string iataCode)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT id, name, latitude, longitude, elevation_ft, iata_code, created_at FROM airports
                    WHERE iata_code=@iataCode and deleted_at IS NULL";
                return connection.QueryFirstOrDefaultAsync<AirportEntity>(sql, new { iataCode });
            }
        }

        public Task<Guid> AddAirportAsync(AirportEntity airport)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO airports(name, latitude, longitude, elevation_ft, iata_code)
                    VALUES (@Name, @Latitude, @Longitude, @ElevationFt, @IataCode) RETURNING id";

                return connection.ExecuteScalarAsync<Guid>(sql, new 
                {  
                    airport.Name,
                    airport.Latitude,
                    airport.Longitude,
                    airport.ElevationFt,
                    airport.IataCode
                });
            }
        }
    }
}
