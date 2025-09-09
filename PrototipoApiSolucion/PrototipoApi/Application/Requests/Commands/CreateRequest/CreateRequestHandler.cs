using MediatR;
using PrototipoApi.Models;
using PrototipoApi.Entities;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PrototipoApi.Infrastructure.RealTime;
using PrototipoApi.Services;

namespace PrototipoApi.Application.Requests.Commands.CreateRequest
{
    public class CreateRequestHandler : IRequestHandler<CreateRequestCommand, RequestDto>
    {
        private readonly IRepository<Request> _requests;
        private readonly IRepository<PrototipoApi.Entities.Building> _buildings;
        private readonly IRepository<Status> _statuses;
        private readonly IExternalBuildingService _externalBuildingService;
        private readonly IRepository<RequestStatusHistory> _requestStatusHistory;
        private readonly ILogger<CreateRequestHandler> _logger;
        private readonly IRealTimeNotifier _realTimeNotifier;

        public CreateRequestHandler(
            IRepository<Request> requests,
            IRepository<PrototipoApi.Entities.Building> buildings,
            IRepository<Status> statuses,
            IExternalBuildingService externalBuildingService,
            IRepository<RequestStatusHistory> requestStatusHistory,
            ILogger<CreateRequestHandler> logger,
            IRealTimeNotifier realTimeNotifier)
        {
            _requests = requests;
            _buildings = buildings;
            _statuses = statuses;
            _externalBuildingService = externalBuildingService;
            _requestStatusHistory = requestStatusHistory;
            _logger = logger;
            _realTimeNotifier = realTimeNotifier;
        }

        public async Task<RequestDto> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            _logger.LogInformation($"Creando request para edificio con código {dto.BuildingCode}");
            // Buscar BuildingId a partir del código
            var building = await _buildings.GetOneAsync(b => b.BuildingCode == dto.BuildingCode);
            if (building == null)
            {
                _logger.LogWarning($"Edificio con código {dto.BuildingCode} no encontrado. Consultando API externa...");
                // Llama a la API externa y crea el edificio si no existe
                building = await _externalBuildingService.GetBuildingByCodeAsync(dto.BuildingCode);
                if (building == null)
                {
                    _logger.LogError($"No se encontró el edificio con código {dto.BuildingCode} en la API externa.");
                    throw new Exception($"No se encontró el edificio con código {dto.BuildingCode} en la API externa.");
                }
                await _buildings.AddAsync(building);
                await _buildings.SaveChangesAsync();
                _logger.LogInformation($"Edificio con código {dto.BuildingCode} creado en la base de datos.");
            }
            // Siempre usar 'Recibido' como estado por defecto
            var status = await _statuses.GetOneAsync(s => s.StatusType == "Recibido");
            if (status == null)
            {
                _logger.LogError("El estado 'Recibido' no existe.");
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
            _logger.LogInformation($"Request creada con id {entity.RequestId}");

            // Registrar historial de estado inicial
            var statusHistory = new RequestStatusHistory
            {
                RequestId = entity.RequestId,
                OldStatusId = null, // No hay estado anterior
                NewStatusId = status.StatusId,
                ChangeDate = DateTime.UtcNow,
                Comment = "Solicitud creada con estado 'Recibido'"
            };
            await _requestStatusHistory.AddAsync(statusHistory);
            await _requestStatusHistory.SaveChangesAsync();

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

            // Notificación SignalR para creación de Request
            var liveDto = new RequestLiveDto(
                entity.RequestId,
                entity.Description,
                (decimal)entity.BuildingAmount,
                (decimal)entity.MaintenanceAmount,
                entity.StatusId,
                status.StatusType,
                entity.BuildingId,
                building.BuildingCode,
                entity.RequestDate
            );
            await _realTimeNotifier.NotifyRequestCreated(liveDto, cancellationToken);

            return created!;
        }
    }
}
