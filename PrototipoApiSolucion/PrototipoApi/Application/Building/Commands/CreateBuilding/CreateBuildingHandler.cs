using AutoMapper;
using MediatR;
using PrototipoApi.Entities;
using PrototipoApi.Models;
using PrototipoApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

public class CreateBuildingHandler : IRequestHandler<CreateBuildingCommand, BuildingDto>
{
    private readonly IRepository<Building> _repository;
    private readonly IMapper _mapper;

    public CreateBuildingHandler(IRepository<Building> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BuildingDto> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Building>(request.Dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<BuildingDto>(entity);
    }
}
