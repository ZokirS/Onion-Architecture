using Application.Queries;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Responses;
using MediatR;
using Shared.DataTransferObjects;

namespace Application.Handlers
{
    internal sealed class GetCompanyHandler : IRequestHandler<GetCompanyQuery, CompanyDto>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        public GetCompanyHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CompanyDto> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            var company = await _repository.Company.GetCompanyAsync(request.id, request.trackChanges) ?? throw new CompanyNotFoundException(request.id);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }
    }
}
