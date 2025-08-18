using System;
using System.Collections.Generic;

namespace Application.Notifications.Email;

public sealed class EmailMessage
{
    public string To { get; }
    public string Subject { get; }
    public string HtmlBody { get; }
    public string TextBody { get; }
    public IReadOnlyDictionary<string, string> Headers { get; }

    public EmailMessage(
        string to,
        string subject,
        string htmlBody,
        string textBody = "",
        IReadOnlyDictionary<string, string>? headers = null)
    {
        To = to ?? throw new ArgumentNullException(nameof(to));
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        HtmlBody = htmlBody ?? throw new ArgumentNullException(nameof(htmlBody));
        TextBody = textBody ?? string.Empty;
        Headers = headers ?? new Dictionary<string, string>();
    }
}
