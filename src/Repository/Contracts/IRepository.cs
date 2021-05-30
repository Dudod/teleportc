using System;
using System.Threading.Tasks;

namespace Repository
{
    public interface IRepository
    {
        Task<AirportEntity> GetAirportAsync(string iataCode);
    }
}