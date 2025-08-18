using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
/// </summary>
public sealed class CreateActivationTokenOnAccountRegisteredHandler
    : INotificationHandler<DomainEventNotification<AccountRegistered>>
{
    private readonly IAccountRepository _accounts;
    private readonly IUnitOfWork _uow;
    private readonly IEmailSender _email;
    private readonly IActivationLinkBuilder _linkBuilder;

    private static readonly TimeSpan TokenTtl = TimeSpan.FromHours(24);

    public CreateActivationTokenOnAccountRegisteredHandler(
        IAccountRepository accounts,
        IUnitOfWork uow,
        IEmailSender email,
        IActivationLinkBuilder linkBuilder)
    {
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _email = email ?? throw new ArgumentNullException(nameof(email));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
    }

    /// <summary>
    /// Generates cryptographic materials, issues a domain token, persists it, and dispatches an HTML+text e-mail.
    /// </summary>
    public async Task Handle(
        DomainEventNotification<AccountRegistered> notification,
        CancellationToken cancellationToken)
    {
        var account = await _accounts.GetByIdAsync(notification.DomainEvent.AccountId, cancellationToken)
                      ?? throw new InvalidOperationException("Account not found.");

        var (raw, hash) = ActivationTokenGenerator.CreateMaterials();

        account.CreateActivationToken(
            hash,
            DateTimeOffset.UtcNow,
            TokenTtl,
            revokeExistingFirst: true);

        await _uow.SaveChangesAsync(cancellationToken);

        var url = _linkBuilder.Build(raw);

        var subject = "Activate your account";
        var html = $"""
            <p>Hello {account.Username.Value},</p>
            <p>Please activate your account by clicking the link below:</p>
            <p><a href="{url}">{url}</a></p>
            """;
        var text = $"Hello {account.Username.Value},{Environment.NewLine}Activate your account:{Environment.NewLine}{url}{Environment.NewLine}";

        await _email.SendAsync(
            new EmailMessage(account.Email.Value, subject, html, text),
            cancellationToken);
    }
}
