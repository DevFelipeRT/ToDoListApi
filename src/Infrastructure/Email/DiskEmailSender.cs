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
/// Writes a simple .eml file under a folder relative to AppContext.BaseDirectory
/// (or an absolute folder if provided).
/// </summary>
public sealed class DiskEmailSender : IEmailSender
{
    private readonly string _outDir;
    private readonly string _defaultFrom;

    /// <param name="outputFolder">
    /// Relative or absolute folder. If relative, it is combined with AppContext.BaseDirectory.
    /// Default: "var/outbox/emails".
    /// </param>
    /// <param name="defaultFrom">Default sender used in the From header.</param>
    public DiskEmailSender(string? outputFolder = "var/outbox/emails",
                           string defaultFrom = "no-reply@example.local")
    {
        if (string.IsNullOrWhiteSpace(outputFolder))
            outputFolder = "var/outbox/emails";

        var baseDir = AppContext.BaseDirectory; // works locally and in containers
        _outDir = Path.IsPathRooted(outputFolder)
            ? outputFolder
            : Path.GetFullPath(Path.Combine(baseDir, outputFolder));

        _defaultFrom = string.IsNullOrWhiteSpace(defaultFrom)
            ? "no-reply@example.local"
            : defaultFrom;

        Directory.CreateDirectory(_outDir);
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));

        var fileName = $"{DateTimeOffset.UtcNow:yyyyMMdd_HHmmssfff}_{Guid.NewGuid():N}.eml";
        var path = Path.Combine(_outDir, fileName);

        var sb = new StringBuilder(capacity: 1024);
        sb.AppendLine($"Date: {DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture)}");
        sb.AppendLine($"From: {_defaultFrom}");
        sb.AppendLine($"To: {SanitizeHeader(message.To)}");
        sb.AppendLine($"Subject: {SanitizeHeader(message.Subject)}");
        sb.AppendLine("MIME-Version: 1.0");

        if (message.Headers is { Count: > 0 })
        {
            foreach (var kv in message.Headers)
            {
                if (string.IsNullOrWhiteSpace(kv.Key)) continue;
                sb.AppendLine($"{kv.Key}: {SanitizeHeader(kv.Value)}");
            }
        }

        var hasHtml = !string.IsNullOrWhiteSpace(message.HtmlBody);
        var body = hasHtml ? message.HtmlBody : (message.TextBody ?? string.Empty);

        sb.AppendLine($"Content-Type: {(hasHtml ? "text/html" : "text/plain")}; charset=utf-8");
        sb.AppendLine("Content-Transfer-Encoding: 8bit");
        sb.AppendLine();
        sb.AppendLine(body);

        // Atomic create/write
        await using var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        var bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetBytes(sb.ToString());
        await fs.WriteAsync(bytes.AsMemory(0, bytes.Length), cancellationToken);
        await fs.FlushAsync(cancellationToken);
    }

    private static string SanitizeHeader(string? value) =>
        (value ?? string.Empty).Replace("\r", " ").Replace("\n", " ").Trim();
}
