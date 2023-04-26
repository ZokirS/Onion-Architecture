namespace Entities.Responses
{
    public sealed class CompanyNotFoundResponse : ApiNotFoundResponse
    {
        public CompanyNotFoundResponse(Guid guid) 
            : base($"Company with id: {guid} not found in db.")
        {
        }
    }
}
