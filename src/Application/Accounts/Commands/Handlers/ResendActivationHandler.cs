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

public sealed class ResendActivationHandler : IRequestHandler<ResendActivationCommand>
{
    private readonly IAccountRepository _accounts;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _email;
    private readonly IActivationLinkBuilder _linkBuilder;

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

        var (raw, hash) = ActivationTokenGenerator.Create(account.Id, DateTimeOffset.Now, TimeSpan.FromHours(2));
        var now = DateTimeOffset.UtcNow;

        account.CreateActivationToken(hash, now, TimeSpan.FromHours(24), revokeExistingFirst: true);
        _accounts.Update(account);

        var link = _linkBuilder.Build(raw.ToString());

        var message = new EmailMessage(
            to: account.Email.ToString(),
            subject: "Activate your account",
            htmlBody: $"<p>To activate your account, click <a href=\"{link}\">here</a>.</p>" +
                      $"<p>If the link does not work, use this token: <strong>{raw}</strong></p>",
            textBody: $"Activate your account: {link}{Environment.NewLine}Token: {raw}");

        await _email.SendAsync(message, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
