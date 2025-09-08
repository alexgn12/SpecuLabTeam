using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace PrototipoApi.Infrastructure.RealTime
{
    public interface IRealTimeNotifier
    {
        Task NotifyTransactionCreated(TransactionLiveDto dto, CancellationToken ct = default);
        Task NotifyTransactionUpdated(TransactionLiveDto dto, CancellationToken ct = default);
        // NUEVO: Métodos para Request
        Task NotifyRequestCreated(RequestLiveDto dto, CancellationToken ct = default);
        Task NotifyRequestUpdated(RequestLiveDto dto, CancellationToken ct = default);
    }

    public class RealTimeNotifier : IRealTimeNotifier
    {
        private readonly IHubContext<LiveHub, ILiveClient> _hub;

        public RealTimeNotifier(IHubContext<LiveHub, ILiveClient> hub)
        {
            _hub = hub;
        }

        public Task NotifyTransactionCreated(TransactionLiveDto dto, CancellationToken ct = default)
            => _hub.Clients.Group("transactions").TransactionCreated(dto);

        public Task NotifyTransactionUpdated(TransactionLiveDto dto, CancellationToken ct = default)
            => _hub.Clients.Group("transactions").TransactionUpdated(dto);

        // NUEVO: Implementación para Request
        public Task NotifyRequestCreated(RequestLiveDto dto, CancellationToken ct = default)
            => _hub.Clients.Group("requests").RequestCreated(dto);

        public Task NotifyRequestUpdated(RequestLiveDto dto, CancellationToken ct = default)
            => _hub.Clients.Group("requests").RequestUpdated(dto);
    }
}
