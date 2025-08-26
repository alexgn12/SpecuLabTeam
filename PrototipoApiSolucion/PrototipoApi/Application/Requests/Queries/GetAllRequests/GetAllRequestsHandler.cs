using MediatR;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.BaseDatos;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using System.Linq.Expressions;

namespace PrototipoApi.Application.Requests.Queries.GetAllRequests
{
    public class GetAllRequestsHandler : IRequestHandler<GetAllRequestsQuery, List<RequestDto>>
    {
        private readonly IRepository<Request> _repository;

        public GetAllRequestsHandler(IRepository<Request> repository)
        {
            _repository = repository;
        }

        public async Task<List<RequestDto>> Handle(GetAllRequestsQuery request, CancellationToken ct)
        {
            var page = request.Page;

            Expression<Func<Request, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Status))
                filter = r => r.Status.StatusType == request.Status;

            Expression<Func<Request, RequestDto>> selector = r => new RequestDto
            {
                RequestId = r.RequestId,
                BuildingAmount = r.BuildingAmount,
                MaintenanceAmount = r.MaintenanceAmount,
                Description = r.Description,
                StatusId = r.StatusId,
                StatusType = r.Status.StatusType,
                BuildingId = r.BuildingId
            };

            Func<IQueryable<Request>, IOrderedQueryable<Request>> orderBy = q =>
                q.OrderByDescending(r => r.RequestDate).ThenBy(r => r.RequestId);

            var items = await _repository.SelectListAsync(
                filter: filter,
                orderBy: orderBy,
                selector: selector,
                skip: (page - 1) * request.Size,
                take: request.Size,
                ct: ct
            );

            return items;
        }
    }
}