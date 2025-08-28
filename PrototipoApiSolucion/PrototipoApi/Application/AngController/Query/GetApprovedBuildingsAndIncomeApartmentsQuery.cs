using MediatR;
using PrototipoApi.Models;

namespace PrototipoApi.Application.AngController.Query
{
    public class GetApprovedBuildingsAndIncomeApartmentsQuery : IRequest<ApprovedBuildingsAndIncomeApartmentsDto>
    {
    }
}
