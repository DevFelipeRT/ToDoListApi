using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Notifications.Email;

/// <summary>
/// Application-level service that provides high-level operations for sending emails.
/// Relies on <see cref="IEmailSender"/> for actual delivery, while encapsulating
/// common application scenarios such as account activation, password reset, and notifications.
/// </summary>
public sealed class EmailService
{
    private readonly IEmailSender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="sender">The email sender infrastructure service.</param>
    public EmailService(IEmailSender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Sends a generic email.
    /// </summary>
    public Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        string textBody = "",
        CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage(to, subject, htmlBody, textBody);
        return _sender.SendAsync(message, cancellationToken);
    }

    /// <summary>
    /// Sends an account activation email with the provided activation link.
    /// </summary>
    public Task SendActivationEmailAsync(
        string to,
        string activationLink,
        CancellationToken cancellationToken = default)
    {
        var subject = "Activate your account";
        var htmlBody = $"<p>Please activate your account by clicking " +
                       $"<a href=\"{activationLink}\">here</a>.</p>";
        var textBody = $"Please activate your account: {activationLink}";

        var message = new EmailMessage(to, subject, htmlBody, textBody);
        return _sender.SendAsync(message, cancellationToken);
    }

    /// <summary>
    /// Sends a password reset email with the provided reset link.
    /// </summary>
    public Task SendPasswordResetEmailAsync(
        string to,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        var subject = "Reset your password";
        var htmlBody = $"<p>You can reset your password by clicking " +
                       $"<a href=\"{resetLink}\">here</a>.</p>";
        var textBody = $"Reset your password: {resetLink}";

        var message = new EmailMessage(to, subject, htmlBody, textBody);
        return _sender.SendAsync(message, cancellationToken);
    }

    /// <summary>
    /// Sends a simple notification email with the given subject and body.
    /// </summary>
    public Task SendNotificationAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage(to, subject, body, body);
        return _sender.SendAsync(message, cancellationToken);
    }
}
