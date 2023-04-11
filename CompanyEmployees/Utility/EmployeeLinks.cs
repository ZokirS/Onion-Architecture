using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Utility
{
    public class EmployeeLinks : IEmployeeLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeDto> _shaper;

        public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _shaper = dataShaper;
        }
        public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto, string fields, Guid companyId, HttpContext httpContext)
        {
            var shapedEmployees = ShapeData(employeesDto, fields);

            if(ShouldGenerateLinks(httpContext))
                return ReturnLinkEmployees(employeesDto, fields, companyId, httpContext, shapedEmployees);

            return ReturnShapedEmployees(shapedEmployees);

        }

        private List<Entity> ShapeData(IEnumerable<EmployeeDto> employeesDto, string fields) =>
            _shaper.ShapeData(employeesDto, fields)
            .Select(e => e.Entity)
            .ToList();

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }

        private LinkResponse ReturnLinkEmployees(IEnumerable<EmployeeDto> employeesDto,
            string fields, Guid companyId, HttpContext httpContext, List<Entity> shapedEmployees)
        {
            var employeeDtoList = employeesDto.ToList();

            for (int i = 0; i < employeeDtoList.Count; i++)
            {
                var employeeLinks = CreateLinksForEmployees(httpContext, companyId, employeeDtoList[i].id, fields);
                shapedEmployees[i].Add("Links", employeeLinks);
            }

            var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);
            var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);

            return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
        }

        private List<Link> CreateLinksForEmployees(HttpContext httpContext, Guid companyId, Guid id, string fields="")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeesForCompany",
                values: new { companyId, id, fields }),
                "self",
                "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext,
                "DeleteEmployeeForCompany", values: new {companyId, id}),
                "delete_employee",
                "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext,
                "UpdateEmployeeForCompany", values: new {companyId, id}),
                "update_employee",
                "PUT"),
                new Link(_linkGenerator.GetUriByAction(httpContext,
                "PartiallyUpdateEmployeeForCompany", values: new {companyId, id}),
                "partially_update_employee",
                "PATCH")
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext, LinkCollectionWrapper<Entity> employeeWrapper)
        {
            employeeWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext,
                "GetEmployeesForCompany", values: new { }),
                "self",
                "GET"));

            return employeeWrapper;
        }

        private LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees) =>
            new() { ShapedEntities = shapedEmployees };
    }
}
