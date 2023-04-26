using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.Extensions;
using CompanyEmployees.Presentation.ModelBinders;
using Entities.Responses;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    public class CompaniesController : ApiControllerBase
    {
        private readonly IServiceManager _service;

        public CompaniesController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets the list of all companies
        /// </summary>
        /// <returns></returns>

        [HttpGet(Name = "GetCompanies")]
        [Authorize(Roles = "Manager")]
        public  IActionResult GetCompanies()
        {
            var baseResult = _service.CompanyService.GetAllCompanies(trackChanges: false);
            var companies = baseResult.GetResult<IEnumerable<CompanyDto>>();
            return Ok(companies);
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate =false)]
        public  IActionResult GetCompany(Guid id)
        {
            var baseResult =  _service.CompanyService.GetCompany(id, false);
            if(!baseResult.Success)
                return ProccessError(baseResult);

            var company = baseResult.GetResult<CompanyDto>();
            return Ok(company);
        }

        [HttpPost(Name = "CreateCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);

            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
            return Ok(companies);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);

            return CreatedAtAction("CompanyCollection", new { result.ids }, result.companies);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateForCompany(Guid id,[FromBody] CompanyForUpdateDto company)
        {
             await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
             return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");

            return Ok();
        }
    }
}
