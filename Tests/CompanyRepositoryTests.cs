using Contracts;
using Entities.Models;
using Moq;

namespace Tests;

public class CompanyRepositoryTests
{
    [Fact]
    public void 
        GetAllCompaniesAsync_ReturnListOfCompanies_WithASingleCompany()
    {
        //Arrange
        var mockRepo = new Mock<ICompanyRepository>();
        mockRepo.Setup(repo => (repo.GetAllCompaniesAsync(false)))
            .Returns(Task.FromResult(GetAllCompanies()));

        var result = mockRepo.Object.GetAllCompaniesAsync(false)
            .GetAwaiter()
            .GetResult()
            .ToList();

        Assert.IsType<List<Company>>(result);
        Assert.Single(result);
    }

    public IEnumerable<Company> GetAllCompanies()
    {
        return new List<Company> {
            new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Country = "United States",
            Address = "908 Woodrow Way"
            }
        };
    }
}