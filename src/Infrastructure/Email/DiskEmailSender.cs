// Infrastructure.Email/DiskEmailSender.cs
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Notifications.Email;

namespace Infrastructure.Email;

/// <summary>
/// Minimal disk-backed email sender with no Hosting dependency.
/// Output directory resolution precedence:
/// 1) constructor parameter (outputFolder)
/// 2) environment variable EMAIL_OUTBOX_DIR
/// 3) default relative folder "var/outbox/emails" (resolved against AppContext.BaseDirectory)
///
/// Default From header:
/// 1) constructor parameter (defaultFrom)
/// 2) environment variable EMAIL_DEFAULT_FROM
/// 3) "no-reply@example.local"
/// </summary>
public sealed class DiskEmailSender : IEmailSender
{
    private readonly string _outDir;
    private readonly string _defaultFrom;

    private const string DefaultRelativeFolder = "var/outbox/emails";
    private const string EnvOutboxVar = "EMAIL_OUTBOX_DIR";
    private const string EnvFromVar = "EMAIL_DEFAULT_FROM";
    private const string CrLf = "\r\n";

    /// <param name="outputFolder">
    /// Absolute or relative folder. If relative, it is combined with AppContext.BaseDirectory.
    /// If null/empty, falls back to EMAIL_OUTBOX_DIR or "var/outbox/emails".
    /// </param>
    /// <param name="defaultFrom">
    /// Default sender used in the From header. If null/empty, falls back to EMAIL_DEFAULT_FROM or "no-reply@example.local".
    /// </param>
    public DiskEmailSender(string? outputFolder = null, string? defaultFrom = null)
    {
        var configuredFolder = !string.IsNullOrWhiteSpace(outputFolder)
            ? outputFolder!
            : Environment.GetEnvironmentVariable(EnvOutboxVar);

        if (string.IsNullOrWhiteSpace(configuredFolder))
            configuredFolder = DefaultRelativeFolder;

        _outDir = Path.IsPathRooted(configuredFolder)
            ? configuredFolder
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredFolder));

        _defaultFrom = !string.IsNullOrWhiteSpace(defaultFrom)
            ? defaultFrom!
            : (Environment.GetEnvironmentVariable(EnvFromVar) ?? "no-reply@example.local");

        Directory.CreateDirectory(_outDir);
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        cancellationToken.ThrowIfCancellationRequested();

        var fileName = $"{DateTimeOffset.UtcNow:yyyyMMdd_HHmmssfff}_{Guid.NewGuid():N}.eml";
        var path = Path.Combine(_outDir, fileName);

        var hasHtml = !string.IsNullOrWhiteSpace(message.HtmlBody);
        var body = hasHtml ? message.HtmlBody! : (message.TextBody ?? string.Empty);

        var sb = new StringBuilder(capacity: 1024);

        // Headers (RFC5322-style CRLF)
        sb.Append("Date: ").Append(DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture)).Append(CrLf);
        sb.Append("From: ").Append(SanitizeHeader(_defaultFrom)).Append(CrLf);
        sb.Append("To: ").Append(SanitizeHeader(message.To)).Append(CrLf);
        sb.Append("Subject: ").Append(SanitizeHeader(message.Subject)).Append(CrLf);
        sb.Append("MIME-Version: 1.0").Append(CrLf);

        if (message.Headers is { Count: > 0 })
        {
            foreach (var kv in message.Headers)
            {
                if (string.IsNullOrWhiteSpace(kv.Key)) continue;
                sb.Append(kv.Key).Append(": ").Append(SanitizeHeader(kv.Value)).Append(CrLf);
            }
        }

        sb.Append("Content-Type: ").Append(hasHtml ? "text/html" : "text/plain").Append("; charset=utf-8").Append(CrLf);
        sb.Append("Content-Transfer-Encoding: 8bit").Append(CrLf);
        sb.Append(CrLf);

        // Body normalized to CRLF
        var normalizedBody = (body ?? string.Empty)
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Replace("\n", CrLf);

        sb.Append(normalizedBody);

        // Atomic create/write
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        await using var fs = new FileStream(
            path,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            4096,
            useAsync: true);

        var bytes = encoding.GetBytes(sb.ToString());
        await fs.WriteAsync(bytes.AsMemory(0, bytes.Length), cancellationToken).ConfigureAwait(false);
        await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private static string SanitizeHeader(string? value) =>
        (value ?? string.Empty).Replace("\r", " ").Replace("\n", " ").Trim();
}
