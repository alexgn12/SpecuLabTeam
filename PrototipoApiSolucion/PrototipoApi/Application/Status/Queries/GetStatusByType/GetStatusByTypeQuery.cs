using MediatR;

namespace PrototipoApi.Application.Status.Queries.GetStatusByType
{
    public class GetStatusByTypeQuery : IRequest<PrototipoApi.Models.StatusDto?>
    {
        public string StatusType { get; }
        public GetStatusByTypeQuery(string statusType)
        {
            StatusType = statusType;
        }
    }
}