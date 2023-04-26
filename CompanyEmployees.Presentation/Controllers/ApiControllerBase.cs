using Entities.ErrorModel;
using Entities.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Presentation.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        public IActionResult ProccessError(ApiBaseResponse baseResponse)
        {
            return baseResponse switch
            {
                ApiNotFoundResponse => NotFound(new ErrorDetails
                {
                    Message = ((ApiNotFoundResponse)baseResponse).Message,
                    StatusCode = StatusCodes.Status400BadRequest
                }),
                ApiBadRequestResponse => BadRequest(new ErrorDetails
                {
                    Message = ((ApiBadRequestResponse)baseResponse).Message,
                    StatusCode = StatusCodes.Status400BadRequest
                }),
                _ => throw new NotImplementedException()
            };
        }
    }
}
