using Application.Commands;

namespace Application.Handlers
{
    public interface IDeleteCompanyHandler
    {
        Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken);
    }
}