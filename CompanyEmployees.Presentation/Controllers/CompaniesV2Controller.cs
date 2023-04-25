using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies")]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [ApiController]
    public class CompaniesV2Controller : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompaniesV2Controller(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _serviceManager.CompanyService
                .GetAllCompaniesAsync(false);

            var comp2 = companies.Select(x => $"{x.Name}");

            return Ok(comp2);
        }
    }
}
