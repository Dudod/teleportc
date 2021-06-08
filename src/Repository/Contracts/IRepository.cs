using System.Threading.Tasks;

namespace Repository
{
    public interface IRepository
    {
        Task<AirportEntity> GetAirportAsync(string iataCode);
        Task<AirportEntity> AddAirportAsync(AirportEntity airport);
        Task<bool> DeleteAirportAsync(string iataCode);
    }
}