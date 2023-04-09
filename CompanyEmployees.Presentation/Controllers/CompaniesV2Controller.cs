using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace CompanyEmployees.Presentation.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/companies")]
    [ApiController]
    public class CompaniesV2Controller : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompaniesV2Controller(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _serviceManager.CompanyService
                .GetAllCompaniesAsync(false);

            var comp2 = companies.Select(x => $"{x.Name}");

            return Ok(comp2);
        }
    }
}
