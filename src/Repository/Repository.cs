using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Repository
{
    public class Repository : IRepository
    {
        private string _connectionString;

        public Repository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public async Task<AirportEntity> GetAirportAsync(string iataCode)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT id, name, latitude, longitude, elevation_ft, iata_code, created_at FROM airports
                    WHERE iata_code=@iataCode and deleted_at IS NULL";

            return await connection.QueryFirstOrDefaultAsync<AirportEntity>(sql, new { iataCode });
        }

        public async Task<Guid> AddAirportAsync(AirportEntity airport)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"INSERT INTO airports(name, latitude, longitude, elevation_ft, iata_code)
                    VALUES (@Name, @Latitude, @Longitude, @Elevation_Ft, @Iata_Code) RETURNING id";

            return await connection.ExecuteScalarAsync<Guid>(sql, new
            {
                airport.Name,
                airport.Latitude,
                airport.Longitude,
                airport.Elevation_Ft,
                airport.Iata_Code
            });
        }
    }
}
