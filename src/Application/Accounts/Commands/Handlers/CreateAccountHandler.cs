using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Entities;
using Domain.Accounts.Repositories;
using Application.Abstractions.Persistence;
using Application.IdentityAccess;

namespace Application.Accounts.Commands.Handlers;

/// <summary>
/// Handles creation of a new Account aggregate orchestrating domain construction and interaction
/// with the external Identity Provider through a neutral gateway abstraction.
/// 
/// Any failure prior to persistence leaves the system unchanged.
/// </summary>
public sealed class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IAccountRepository _accounts;
    private readonly IIdentityGateway _identityGateway;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountHandler(
        IAccountRepository accounts,
        IIdentityGateway identityGateway,
        IUnitOfWork unitOfWork)
    {
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _identityGateway = identityGateway ?? throw new ArgumentNullException(nameof(identityGateway));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Processes the create account command and returns the created aggregate identifier.
    /// </summary>
    public async Task<Guid> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var email = new AccountEmail(command.Email);
        var username = new AccountUsername(command.Username);
        var name = new AccountName(command.Name);

        await EnsureUniquenessAsync(email, username, cancellationToken);

        var credentialId = await ProvisionExternalIdentityAsync(email, username, command.PlainPassword, cancellationToken);

        var account = Account.Create(email, username, name);
        account.LinkToCredentials(credentialId);
        _accounts.Add(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return account.Id.Value;
    }

    /// <summary>
    /// Ensures that the provided email and username are not already registered.
    /// Throws <see cref="InvalidOperationException"/> if any uniqueness constraint is violated.
    /// </summary>
    private async Task EnsureUniquenessAsync(AccountEmail email, AccountUsername username, CancellationToken cancellationToken)
    {
        if (await _accounts.ExistsByEmailAsync(email, cancellationToken))
            throw new InvalidOperationException("Email already registered.");

        if (await _accounts.ExistsByUsernameAsync(username, cancellationToken))
            throw new InvalidOperationException("Username already registered.");
    }

    /// <summary>
    /// Creates an external identity user and returns its credential identifier.
    /// Exceptions bubble up to abort the process before persistence.
    /// </summary>
    private async Task<CredentialId> ProvisionExternalIdentityAsync(AccountEmail email, AccountUsername username, string plainPassword, CancellationToken cancellationToken)
    {
        return await _identityGateway.CreateUserAsync(email.Value, username.Value, plainPassword, cancellationToken);
    }
}