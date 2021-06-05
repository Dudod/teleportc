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

            var sql = @"SELECT ""id"", ""name"", ""latitude"", ""longitude"", ""elevation_ft"", ""iata_code"", ""created_at"" FROM public.""airports""
                WHERE ""iata_code"" = @IataCode and ""deleted_at"" IS NULL;";

            return await connection.QueryFirstOrDefaultAsync<AirportEntity>(sql, new { iataCode });
        }

        public async Task<AirportEntity> AddAirportAsync(AirportEntity airport)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"INSERT INTO public.""airports""(""name"", ""latitude"", ""longitude"", ""elevation_ft"", ""iata_code"")
                VALUES(@Name, @Latitude, @Longitude, @Elevation_Ft, @Iata_Code)
                ON CONFLICT DO NOTHING;
                SELECT ""id"", ""name"", ""latitude"", ""longitude"", ""elevation_ft"", ""iata_code"", ""created_at"" FROM public.""airports""
                WHERE ""iata_code"" = @Iata_Code and ""deleted_at"" IS NULL;";

            try
            {
                return await connection.QueryFirstOrDefaultAsync<AirportEntity>(sql, new
                {
                    airport.Name,
                    airport.Latitude,
                    airport.Longitude,
                    airport.Elevation_Ft,
                    airport.Iata_Code
                });
            }
            catch(Exception e)
            {
                return default;
            }
        }

        public async Task<bool> DeleteAirportAsync(string iataCode)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"UPDATE public.""airports"" SET ""deleted_at"" = timezone('utc'::text, now())
                WHERE ""iata_code"" = @IataCode and ""deleted_at"" IS NULL;";

            return await connection.ExecuteAsync(sql, new { iataCode }) > 0;
        }
    }
}
