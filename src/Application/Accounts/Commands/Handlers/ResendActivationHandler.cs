using Application.Accounts.Services;
using Domain.Accounts.Entities;
using Domain.Accounts.Repositories;
using MediatR;

namespace Application.Accounts.Commands.Handlers;

public sealed class ResendActivationHandler : IRequestHandler<ResendActivationCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IActivationTokenRepository _tokens;
    private readonly IEmailSender _email;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IAppUrls _urls;

    public ResendActivationHandler(
        IAccountRepository accountRepository, IActivationTokenRepository tokens,
        IEmailSender email, IUnitOfWork uow, IClock clock, IAppUrls urls)
    {
        _accountRepository = accountRepository; _tokens = tokens; _email = email; _uow = uow; _clock = clock; _urls = urls;
    }

    public async Task<Unit> Handle(ResendActivationCommand cmd, CancellationToken ct)
    {
        var user = await _accountRepository.FindByEmailAsync(cmd.Email, ct);
        if (user is null || user.EmailConfirmed) return Unit.Value; // do nothing

        // Optionally throttle: do not issue if a valid token exists recently
        await _tokens.InvalidateAllAsync(user.Id, ct);

        var (raw, hash) = ActivationTokenGenerator.Create();
        var expiresAt = _clock.UtcNow().AddHours(24);
        await _tokens.AddAsync(new ActivationToken(user.Id, hash, expiresAt), ct);
        await _uow.SaveChangesAsync(ct);

        var link = _urls.BuildActivationLink(raw);
        await _email.SendAsync(user.Email, "Activate your account", EmailTemplates.Activation(link), ct);
        return Unit.Value;
    }
}
