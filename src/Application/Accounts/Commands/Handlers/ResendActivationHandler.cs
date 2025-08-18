using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Persistence;
using Application.Accounts.Abstractions;
using Application.Accounts.Services;
using Application.Notifications.Email;
using Domain.Accounts.Repositories;
using Domain.Accounts.ValueObjects;
using MediatR;

namespace Application.Accounts.Commands.Handlers;

/// <summary>
/// Resends an activation token for a pending account and dispatches the e-mail.
/// </summary>
public sealed class ResendActivationHandler : IRequestHandler<ResendActivationCommand>
{
    private readonly IAccountRepository _accounts;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _email;
    private readonly IActivationLinkBuilder _linkBuilder;

    private static readonly TimeSpan TokenTtl = TimeSpan.FromHours(2);

    public ResendActivationHandler(
        IAccountRepository accounts,
        IUnitOfWork unitOfWork,
        IEmailSender email,
        IActivationLinkBuilder linkBuilder)
    {
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _email = email ?? throw new ArgumentNullException(nameof(email));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
    }

    public async Task Handle(ResendActivationCommand command, CancellationToken cancellationToken)
    {
        var email = AccountEmail.FromString(command.Email);

        var account = await _accounts.GetForActivationByEmailAsync(email, cancellationToken);
        if (account is null || account.ActivatedAt.HasValue) return;

        (string raw, string hash) = ActivationTokenGenerator.CreateMaterials();

        var now = DateTimeOffset.UtcNow;
        account.CreateActivationToken(hash, now, TokenTtl, revokeExistingFirst: true);
        _accounts.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var link = _linkBuilder.Build(raw);

        var message = new EmailMessage(
            to: account.Email.Value,
            subject: "Activate your account",
            htmlBody: $"""
                <p>Hello {account.Username.Value},</p>
                <p>Please activate your account by clicking the link below:</p>
                <p><a href="{link}">{link}</a></p>
                """,
            textBody: $"Hello {account.Username.Value},{Environment.NewLine}Activate your account:{Environment.NewLine}{link}{Environment.NewLine}");

        await _email.SendAsync(message, cancellationToken);
    }
}
