using MediatR;
using System;

namespace PrototipoApi.Application.Requests.Commands.PatchRequest
{
    public class PatchRequestCommand : IRequest<bool?>
    {
        public int RequestId { get; }
        public int StatusId { get; }
        public string? Comment { get; }
        public DateTime? ChangeDate { get; }
        public PatchRequestCommand(int requestId, int statusId, string? comment = null, DateTime? changeDate = null)
        {
            RequestId = requestId;
            StatusId = statusId;
            Comment = comment;
            ChangeDate = changeDate;
        }
    }
}
