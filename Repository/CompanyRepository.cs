using Entities.Models;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext) {}

        public  IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
             FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToList();

        public  Company GetCompany(Guid companyId, bool trackChanges) =>
             FindByCondition(c => c.Id.Equals(companyId), trackChanges)
            .SingleOrDefault();

        public void CreateCompany(Company company) => Create(company);

        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
            await FindByCondition(x => ids.Contains(x.Id), trackChanges)
            .ToListAsync();

        public void DeleteCompany(Company company) => Delete(company);
        
    }
}
