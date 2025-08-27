using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.IdentityAccess.Services;
using Application.IdentityAccess.Contracts;
using Domain.Accounts.Events;
using Application.Abstractions.Messaging;
using Application.Notifications.Email;
using Application.IdentityAccess;
using Domain.Accounts.Repositories;

namespace Application.Accounts.EventHandlers;

/// <summary>
/// Handles the <see cref="AccountRegistered"/> domain event by generating an activation token
/// and triggering the email activation process.
/// </summary>
public sealed class SendActivationEmailOnAccountRegisteredHandler
    : INotificationHandler<DomainEventNotification<AccountRegistered>>
{
    private readonly IIdentityGateway _identityGateway;
    private readonly IAccountRepository _accountRepository;
    private readonly IEmailActivationService _activationService;
    private readonly EmailService _emailService;
    private readonly IActivationUrlService _activationUrlService;

    public SendActivationEmailOnAccountRegisteredHandler(
        IIdentityGateway identityGateway,
        IAccountRepository accountRepository,
        IEmailActivationService activationService,
        EmailService emailService,
        IActivationUrlService activationUrlService)
    {
        _identityGateway = identityGateway;
        _accountRepository = accountRepository;
        _activationService = activationService;
        _emailService = emailService;
        _activationUrlService = activationUrlService;
    }

    public async Task Handle(
        DomainEventNotification<AccountRegistered> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var account = await _accountRepository
            .GetByIdAsync(domainEvent.AccountId, cancellationToken);

        if (account is null)
        {
            throw new InvalidOperationException($"Account with ID {domainEvent.AccountId} not found.");
        }

        var credentialId = account.CredentialId;

        if (credentialId is null)
        {
            throw new InvalidOperationException($"CredentialId not found for account {domainEvent.AccountId}.");
        }

        var userDto = await _identityGateway
            .FindByCredentialIdAsync(credentialId, cancellationToken);

        if (userDto is null)
        {
            throw new InvalidOperationException($"User with Credential ID {credentialId} not found in identity provider.");
        }

        ActivationToken token = await _activationService
            .GenerateEmailActivationTokenAsync(credentialId, cancellationToken);

        var activationLink = _activationUrlService.BuildActivationUrl(token, credentialId);

        await _emailService.SendActivationEmailAsync(
            userDto.Email,
            activationLink,
            cancellationToken);
    }
}
