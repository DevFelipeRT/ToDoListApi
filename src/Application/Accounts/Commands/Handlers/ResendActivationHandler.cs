using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Persistence;
using Application.Notifications.Email;
using Application.IdentityAccess;
using Application.IdentityAccess.Contracts;
using Application.IdentityAccess.Services;
using Domain.Accounts.Repositories;
using Domain.Accounts.ValueObjects;
using MediatR;

namespace Application.Accounts.Commands.Handlers;

/// <summary>
/// Resends an activation token for a non-active account by requesting a new activation token
/// from the external identity provider and dispatching an e-mail to the account owner.
/// </summary>
public sealed class ResendActivationHandler : IRequestHandler<ResendActivationCommand>
{
    private readonly IAccountRepository _accounts;
    private readonly IIdentityGateway _identityGateway;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly IEmailActivationService _emailActivationService;
    private readonly IActivationUrlService _activationUrlService;

    public ResendActivationHandler(
        IAccountRepository accounts,
        IIdentityGateway identityGateway,
        IUnitOfWork unitOfWork,
        IEmailSender emailSender,
        IEmailActivationService activationService,
        IActivationUrlService activationUrlService)
    {
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _identityGateway = identityGateway ?? throw new ArgumentNullException(nameof(identityGateway));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        _emailActivationService = activationService ?? throw new ArgumentNullException(nameof(activationService));
        _activationUrlService = activationUrlService ?? throw new ArgumentNullException(nameof(activationUrlService));
    }

    /// <inheritdoc />
    public async Task Handle(ResendActivationCommand command, CancellationToken cancellationToken)
    {
        var account = await LoadPendingAccountAsync(command.Email, cancellationToken);
        if (account is null)
            throw new InvalidOperationException("No pending account found for the specified e-mail address.");

        if (account.CredentialId is null)
            throw new InvalidOperationException("Account is not linked to credentials.");

        // Opcional: garantir que o usuário existe no IdP (mantido como no original)
        var userDto = await _identityGateway.FindByCredentialIdAsync(account.CredentialId, cancellationToken);
        if (userDto is null)
            throw new InvalidOperationException($"User with Credential ID {account.CredentialId} not found in identity provider.");

        // Gera novo token
        ActivationToken token = await GenerateActivationTokenAsync(account, cancellationToken);

        // Constrói URL de ativação via serviço (protege o CredentialId e compõe a URL)
        var link = _activationUrlService.BuildActivationUrl(token, account.CredentialId);

        // Envia e-mail
        var message = BuildEmailMessage(account.Email.Value, account.Username.Value, link);
        await _emailSender.SendAsync(message, cancellationToken);
    }

    /// <summary>
    /// Loads the account for the provided email when it exists and is not active.
    /// Returns null otherwise.
    /// </summary>
    private async Task<Domain.Accounts.Entities.Account?> LoadPendingAccountAsync(string rawEmail, CancellationToken ct)
    {
        var email = new AccountEmail(rawEmail);
        var account = await _accounts.GetByEmailAsync(email, ct);
        if (account is null) return null;
        if (account.IsActive) return null;
        return account;
    }

    /// <summary>
    /// Requests a new activation token from the identity provider for the specified account.
    /// Persists any domain changes prior to returning the token.
    /// </summary>
    private async Task<ActivationToken> GenerateActivationTokenAsync(Domain.Accounts.Entities.Account account, CancellationToken ct)
    {
        if (account.CredentialId is null)
            throw new InvalidOperationException("Account is not linked to credentials.");

        var token = await _emailActivationService.GenerateEmailActivationTokenAsync(account.CredentialId, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return token;
    }

    /// <summary>
    /// Builds the activation e-mail message.
    /// </summary>
    private static EmailMessage BuildEmailMessage(string to, string username, string link)
    {
        var subject = "Activate your account";
        var html = $"""
            <p>Hello {username},</p>
            <p>Please activate your account by clicking the link below:</p>
            <p><a href="{link}">{link}</a></p>
            """;
        var text = $"Hello {username}{Environment.NewLine}Activate your account:{Environment.NewLine}{link}{Environment.NewLine}";
        return new EmailMessage(to, subject, html, text);
    }
}
