using System.Threading.Tasks;
using PrototipoApi.Entities;

namespace PrototipoApi.Services
{
    public interface IExternalApartmentService
    {
        Task<Apartment?> GetApartmentByCodeAsync(string apartmentCode);
    }
}
