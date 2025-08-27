using System;
using System.Collections.Generic;
using Application.IdentityAccess.Contracts;
using Application.IdentityAccess.Services;
using Application.Abstractions.Links;
using Application.IdentityAccess.Links;
using Domain.Accounts.ValueObjects;
using Microsoft.Extensions.Options;

namespace Infrastructure.IdentityAccess.Services;

/// <inheritdoc cref="IActivationUrlService"/>
public sealed class ActivationUrlService : IActivationUrlService
{
    private readonly ILinkBuilder _linkBuilder;
    private readonly IUrlCrypto _urlCrypto;
    private readonly ActivationLinkOptions _options;

    public ActivationUrlService(
        ILinkBuilder linkBuilder,
        IUrlCrypto urlCrypto,
        IOptionsSnapshot<ActivationLinkOptions> options)
    {
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
        _urlCrypto   = urlCrypto   ?? throw new ArgumentNullException(nameof(urlCrypto));
        _options     = options?.Value ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
            throw new InvalidOperationException("ActivationLinkOptions.BaseUrl is required.");

        _options.Path ??= string.Empty;
    }

    /// <inheritdoc />
    public string BuildActivationUrl(ActivationToken token, CredentialId credentialId)
    {
        if (token is null) throw new ArgumentNullException(nameof(token));

        if (string.IsNullOrWhiteSpace(token.Value))
            throw new ArgumentException("Token value must be provided.", nameof(token));

        if (credentialId is null) throw new ArgumentNullException(nameof(credentialId));

        var rawId = credentialId.Value;

        var query = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [_options.TokenKey] = token.Value,
            [_options.IdKey]    = _urlCrypto.Protect(rawId)
        };

        return _linkBuilder.Build(_options, query);
    }
}
