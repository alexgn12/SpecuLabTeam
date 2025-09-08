namespace PrototipoApi.Infrastructure.RealTime
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using System;
    using System.Linq;

    public interface ILiveClient
    {
        Task TransactionCreated(TransactionLiveDto dto);
        Task TransactionUpdated(TransactionLiveDto dto);
        // NUEVO: Eventos para Request
        Task RequestCreated(RequestLiveDto dto);
        Task RequestUpdated(RequestLiveDto dto);
        // Puedes agregar más eventos aquí (BudgetChanged, etc.)
    }

    public record TransactionLiveDto(
        int TransactionId,
        string Type,        // "INGRESO" | "GASTO"
        decimal Amount,
        string Description,
        DateTime UtcCreated
    );

    // NUEVO: DTO para notificaciones de Request
    public record RequestLiveDto(
        int RequestId,
        string Description,
        decimal BuildingAmount,
        decimal MaintenanceAmount,
        int StatusId,
        string StatusType,
        int BuildingId,
        string BuildingCode,
        DateTime RequestDate
    );

    public class LiveHub : Hub<ILiveClient>
    {
        // Suscripción a grupos por query: /hubs/live?topics=transactions,requests
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var topicsCsv = http?.Request.Query["topics"].ToString() ?? "";
            var topics = topicsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var t in topics)
                await Groups.AddToGroupAsync(Context.ConnectionId, t);

            await base.OnConnectedAsync();
        }

        public Task JoinTopics(string[] topics) =>
            Task.WhenAll(topics.Select(t => Groups.AddToGroupAsync(Context.ConnectionId, t)));

        public Task LeaveTopics(string[] topics) =>
            Task.WhenAll(topics.Select(t => Groups.RemoveFromGroupAsync(Context.ConnectionId, t)));
    }
}
