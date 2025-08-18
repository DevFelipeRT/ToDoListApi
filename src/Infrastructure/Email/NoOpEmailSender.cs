using Application.Notifications.Email;

namespace Infrastructure.Email;

public sealed class NoOpEmailSender : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
