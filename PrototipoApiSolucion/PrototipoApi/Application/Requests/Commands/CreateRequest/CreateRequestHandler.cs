using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.BaseDatos;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Request = PrototipoApi.Entities.Request;
using PrototipoApi.Application.Requests.Commands.CreateRequest;
using PrototipoApi.Services;
using PrototipoApi.Logging;

// Handler que usa CreateRequestDto, pero mantiene el patrón MediatR con CreateRequestCommand
public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, RequestDto>
{
    private readonly IRepository<Request> _requests;
    private readonly IRepository<Building> _buildings;
    private readonly IRepository<Status> _statuses;
    private readonly IExternalBuildingService _externalBuildingService;
    private readonly ILoguer _loguer;

    public CreateRequestCommandHandler(
        IRepository<Request> requests,
        IRepository<Building> buildings,
        IRepository<Status> statuses,
        IExternalBuildingService externalBuildingService,
        ILoguer loguer)
    {
        _requests = requests;
        _buildings = buildings;
        _statuses = statuses;
        _externalBuildingService = externalBuildingService;
        _loguer = loguer;
    }

    public async Task<RequestDto> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        _loguer.LogInfo($"Creando request para edificio con código {dto.BuildingCode}");
        // Buscar BuildingId a partir del código
        var building = await _buildings.GetOneAsync(b => b.BuildingCode == dto.BuildingCode);
        if (building == null)
        {
            _loguer.LogWarning($"Edificio con código {dto.BuildingCode} no encontrado. Consultando API externa...");
            // Llama a la API externa y crea el edificio si no existe
            building = await _externalBuildingService.GetBuildingByCodeAsync(dto.BuildingCode);
            if (building == null)
            {
                _loguer.LogError($"No se encontró el edificio con código {dto.BuildingCode} en la API externa.");
                throw new Exception($"No se encontró el edificio con código {dto.BuildingCode} en la API externa.");
            }
            await _buildings.AddAsync(building);
            await _buildings.SaveChangesAsync();
            _loguer.LogInfo($"Edificio con código {dto.BuildingCode} creado en la base de datos.");
        }
        // Siempre usar 'Recibido' como estado por defecto
        var status = await _statuses.GetOneAsync(s => s.StatusType == "Recibido");
        if (status == null)
        {
            _loguer.LogError("El estado 'Recibido' no existe.");
            throw new Exception("El estado 'Recibido' no existe.");
        }

        var entity = new Request
        {
            Description = dto.Description,
            BuildingAmount = dto.BuildingAmount,
            MaintenanceAmount = dto.MaintenanceAmount,
            BuildingId = building.BuildingId,
            StatusId = status.StatusId,
            RequestDate = DateTime.UtcNow
        };

        await _requests.AddAsync(entity);
        await _requests.SaveChangesAsync();
        _loguer.LogInfo($"Request creada con id {entity.RequestId}");

        // Proyección directa a DTO
        var created = await _requests.SelectOneAsync<RequestDto>(
            r => r.RequestId == entity.RequestId,
            r => new RequestDto
            {
                RequestId = r.RequestId,
                BuildingAmount = r.BuildingAmount,
                MaintenanceAmount = r.MaintenanceAmount,
                Description = r.Description,
                StatusId = r.StatusId,
                StatusType = r.Status.StatusType,
                BuildingId = r.BuildingId
            },
            cancellationToken
        );

        return created!;
    }
}
