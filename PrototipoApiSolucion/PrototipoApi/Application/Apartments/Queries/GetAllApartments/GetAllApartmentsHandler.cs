using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace PrototipoApi.Application.Apartments.Queries.GetAllApartments
{
    public class GetAllApartmentsHandler : IRequestHandler<GetAllApartmentsQuery, List<ApartmentDto>>
    {
        private readonly IRepository<Apartment> _apartments;
        private readonly ILogger<GetAllApartmentsHandler> _logger;
        public GetAllApartmentsHandler(IRepository<Apartment> apartments, ILogger<GetAllApartmentsHandler> logger)
        {
            _apartments = apartments;
            _logger = logger;
        }
        public async Task<List<ApartmentDto>> Handle(GetAllApartmentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Handler: Obteniendo apartamentos. Página: {request.Page}, Tamaño: {request.Size}, Orden: {request.OrderBy}, Desc: {request.Desc}");

                Func<IQueryable<Apartment>, IOrderedQueryable<Apartment>>? orderBy = null;
                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    if (request.OrderBy.Equals("CreatedDate", StringComparison.OrdinalIgnoreCase))
                    {
                        orderBy = q => request.Desc ? q.OrderByDescending(a => a.CreatedDate) : q.OrderBy(a => a.CreatedDate);
                    }
                }

                int skip = (request.Page - 1) * request.Size;
                int take = request.Size;

                var result = await _apartments.SelectListAsync<ApartmentDto>(
                    null,
                    orderBy,
                    a => new ApartmentDto
                    {
                        ApartmentId = a.ApartmentId,
                        ApartmentCode = a.ApartmentCode,
                        ApartmentDoor = a.ApartmentDoor,
                        ApartmentFloor = a.ApartmentFloor,
                        ApartmentPrice = a.ApartmentPrice,
                        NumberOfRooms = a.NumberOfRooms,
                        NumberOfBathrooms = a.NumberOfBathrooms,
                        BuildingCode = a.Building.BuildingCode,
                        HasLift = a.HasLift,
                        HasGarage = a.HasGarage,
                        CreatedDate = a.CreatedDate
                    },
                    skip,
                    take,
                    cancellationToken
                );
                _logger.LogInformation($"Handler: {result.Count} apartamentos recuperados");
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAllApartmentsHandler");
                throw;
            }
        }
    }
}
