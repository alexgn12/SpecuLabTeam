using MediatR;
using System.Collections.Generic;

namespace PrototipoApi.Application.AngController.Query.GetRequestResumen
{
    public record GetRequestsResumenQuery() : IRequest<Dictionary<string, int>>;
}