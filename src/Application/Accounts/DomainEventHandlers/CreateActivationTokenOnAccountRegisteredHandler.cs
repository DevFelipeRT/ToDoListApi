using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Notifications.Email;
using Application.Accounts.Services;
using Application.Accounts.Abstractions;
using Domain.Accounts.Events;
using Domain.Accounts.Repositories;

namespace Application.Accounts.DomainEventHandlers;

/// <summary>
/// Handles <see cref="AccountRegistered"/> by issuing an activation token and sending an e-mail via <see cref="IEmailSender"/>.
/// Persistence occurs first; the e-mail dispatch happens afterwards and does not block or rollback the saved state.
/// </summary>
public sealed class CreateActivationTokenOnAccountRegisteredHandler
    : INotificationHandler<DomainEventNotification<AccountRegistered>>
{
    private readonly IAccountRepository _accounts;
    private readonly IUnitOfWork _uow;
    private readonly IEmailSender _email;
    private readonly IActivationLinkBuilder _linkBuilder;
    private readonly ILogger<CreateActivationTokenOnAccountRegisteredHandler> _logger;

    private static readonly TimeSpan TokenTtl = TimeSpan.FromHours(24);

    public CreateActivationTokenOnAccountRegisteredHandler(
        IAccountRepository accounts,
        IUnitOfWork uow,
        IEmailSender email,
        IActivationLinkBuilder linkBuilder,
        ILogger<CreateActivationTokenOnAccountRegisteredHandler> logger)
    {
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _email = email ?? throw new ArgumentNullException(nameof(email));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates cryptographic materials, issues a domain token, persists it, and dispatches an HTML+text e-mail.
    /// E-mail send errors are logged and do not revert the committed changes.
    /// </summary>
    public async Task Handle(
        DomainEventNotification<AccountRegistered> notification,
        CancellationToken cancellationToken)
    {
        var account = await _accounts
            .GetByIdAsync(notification.DomainEvent.AccountId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Account not found.");

        var (raw, hash) = ActivationTokenGenerator.CreateMaterials();

        account.CreateActivationToken(
            hash,
            DateTimeOffset.UtcNow,
            TokenTtl,
            revokeExistingFirst: true);

        // Persist the new activation token BEFORE attempting to send the e-mail.
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var url = _linkBuilder.Build(raw);

        var subject = "Activate your account";
        var html = $"""
            <p>Hello {account.Username.Value},</p>
            <p>Please activate your account by clicking the link below:</p>
            <p><a href="{url}">{url}</a></p>
            """;
        var text = $"Hello {account.Username.Value},{Environment.NewLine}Activate your account:{Environment.NewLine}{url}{Environment.NewLine}";

        // E-mail send should not rollback persistence; log and continue on failure.
        try
        {
            await _email
                .SendAsync(new EmailMessage(account.Email.Value, subject, html, text), cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation(
                "Activation e-mail queued/sent for AccountId={AccountId}, Username={Username}.",
                account.Id.Value, account.Username.Value);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(
                ex,
                "Failed to send activation e-mail for AccountId={AccountId}, Username={Username}.",
                account.Id.Value, account.Username.Value);
            // Intentionally not rethrowing: persistence succeeded and should not be undone.
            // Consider a background retry/outbox for production scenarios.
        }
    }
}
