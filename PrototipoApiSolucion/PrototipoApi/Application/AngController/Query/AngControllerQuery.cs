using MediatR;

namespace PrototipoApi.Application.AngController.Query
{
    public record AngContollerQuery() : IRequest<Dictionary<string, int>>;
}
