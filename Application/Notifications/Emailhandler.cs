using Contracts;
using MediatR;

namespace Application.Notifications
{
    public sealed class Emailhandler : INotificationHandler<CompanyDeletedNotification>
    {
        private readonly ILoggerManager _logger;
        public Emailhandler(ILoggerManager logger) => _logger = logger;

        public async Task Handle(CompanyDeletedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogWarn($"Delete action for company with id:" +
                $"{notification.Id} has occured.");

            await Task.CompletedTask;
        }
    }
}
