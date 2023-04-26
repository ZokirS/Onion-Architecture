using CompanyEmployees.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Contracts;
using Shared.DataTransferObjects;
using System.Security.Cryptography.X509Certificates;

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
        public  IActionResult GetCompanies()
        {
            var baseResult =  _serviceManager.CompanyService
            .GetAllCompanies(false);

            var companies = baseResult.GetResult<IEnumerable<CompanyDto>>().Select(x=>$"{x.Name}");

            return Ok(companies);
        }
    }
}
