using Shared.DataTransferObjects;

namespace Application.Handlers
{
    internal interface IGetCompaniesHandler
    {
        Task<IEnumerable<CompanyDto>> Handle(GetCompaniesHandler request, CancellationToken cancellationToken);
    }
}