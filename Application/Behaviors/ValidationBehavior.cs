using Entities.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Application.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
                    _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var errorDictionaries = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .GroupBy(
                x => x.PropertyName.Substring(x.PropertyName.IndexOf('.') + 1),
                x => x.ErrorMessage,
                (propertyName, errorMessage) => new
                {
                    Key = propertyName,
                    Values = errorMessage.Distinct().ToArray()
                })
                .ToDictionary(x => x.Key, x => x.Values);

            if (errorDictionaries.Any())
                throw new ValidationAppException(errorDictionaries);

            return await next();
        }
    }
}
