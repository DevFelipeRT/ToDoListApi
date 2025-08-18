using System.Threading;
using System.Threading.Tasks;

namespace Application.Notifications.Email;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
