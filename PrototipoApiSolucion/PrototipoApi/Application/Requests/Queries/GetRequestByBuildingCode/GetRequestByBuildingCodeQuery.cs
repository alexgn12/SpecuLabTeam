using MediatR;
using PrototipoApi.Models;

namespace PrototipoApi.Application.Requests.Queries.GetRequestByBuildingCode
{
    public record GetRequestByBuildingCodeQuery(string BuildingCode) : IRequest<RequestDto?>;
}
