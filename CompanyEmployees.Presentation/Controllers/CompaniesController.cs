using Application.Commands;
using Application.Handlers;
using Application.Notifications;
using Application.Queries;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.Extensions;
using CompanyEmployees.Presentation.ModelBinders;
using Entities.Responses;
using Marvin.Cache.Headers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    public class CompaniesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IServiceManager _service;
        private readonly IPublisher _publisher;

        public CompaniesController(ISender sender, IServiceManager serviceManager, IPublisher publisher)
        {
            _sender = sender;
            _service = serviceManager;
            _publisher = publisher;
        }

        /// <summary>
        /// Gets the list of all companies
        /// </summary>
        /// <returns></returns>

        [HttpGet(Name = "GetCompanies")]
        [Authorize(Roles = "Manager")]
        public async  Task<IActionResult> GetCompanies()
        {
            var companies = await _sender.Send(new GetCompaniesQuery(trackChanges: false));
            return Ok(companies);
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate =false)]
        public  async Task<IActionResult> GetCompany(Guid id)
        {
          /*  var baseResult = await _service.CompanyService.GetCompanyAsync(id, false);
            if(!baseResult.Success)
                return ProccessError(baseResult);

            var company = baseResult.GetResult<CompanyDto>();*/

            var company = await _sender.Send(new GetCompanyQuery(id, trackChanges: false));
            return Ok(company);
        }

        [HttpPost(Name = "CreateCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto companyForCreationDto)
        {
            if (companyForCreationDto is null)
                return BadRequest("CompanyForCreationDto object is null");

            var company = await _sender.Send(new CreateCompanyCommand(companyForCreationDto));

            return CreatedAtRoute("CompanyById", new { id = company.Id }, company);
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
            await _publisher.Publish(new CompanyDeletedNotification(id, TrackChanges: false));

            return NoContent();
        }

        /*[HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateForCompany(Guid id,[FromBody] CompanyForUpdateDto company)
        {
             await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
             return NoContent();
        }*/

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCompany(Guid id, CompanyForUpdateDto companyForUpdateDto)
        {
            if (companyForUpdateDto is null)
                return BadRequest("CompanyForUpdateDto object is null");

            await _sender.Send(new UpdateCompanyCommand(id, companyForUpdateDto, TrackChanges: true));

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
