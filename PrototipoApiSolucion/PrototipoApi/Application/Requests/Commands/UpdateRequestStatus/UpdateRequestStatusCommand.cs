using MediatR;
using System;

namespace PrototipoApi.Application.Requests.Commands.UpdateRequestStatus
{
    public class UpdateRequestStatusCommand : IRequest<bool?>
    {
        public int RequestId { get; }
        public string StatusType { get; }
        public string? Comment { get; }
        public DateTime? ChangeDate { get; }
        public UpdateRequestStatusCommand(int requestId, string statusType, string? comment = null, DateTime? changeDate = null)
        {
            RequestId = requestId;
            StatusType = statusType;
            Comment = comment;
            ChangeDate = changeDate;
        }
    }
}
