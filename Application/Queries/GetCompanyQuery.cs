using MediatR;
using Shared.DataTransferObjects;

namespace Application.Queries
{
    public sealed record GetCompanyQuery(Guid id, bool trackChanges) : IRequest<CompanyDto>;
}
