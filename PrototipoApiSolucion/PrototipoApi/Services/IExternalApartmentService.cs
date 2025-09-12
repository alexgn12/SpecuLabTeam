using System.Threading.Tasks;
using PrototipoApi.Models;

namespace PrototipoApi.Services
{
    public interface IExternalApartmentService
    {
        Task<ApartmentDto?> GetApartmentByCodeAsync(string apartmentCode);
    }
}
