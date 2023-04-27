using MediatR;
using Shared.DataTransferObjects;

namespace Application.Queries
{
    public sealed record GetCompaniesQuery(bool trackChanges): IRequest<IEnumerable<CompanyDto>>;
}
