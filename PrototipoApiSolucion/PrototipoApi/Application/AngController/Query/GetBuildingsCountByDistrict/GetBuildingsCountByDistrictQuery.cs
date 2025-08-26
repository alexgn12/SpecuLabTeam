using MediatR;
using System.Collections.Generic;
using PrototipoApi.Models;

namespace PrototipoApi.Application.AngController.Query.GetBuildingsCountByDistrict
{
    public record GetBuildingsCountByDistrictQuery() : IRequest<List<BuildingsCountByDistrictDto>>;
}
