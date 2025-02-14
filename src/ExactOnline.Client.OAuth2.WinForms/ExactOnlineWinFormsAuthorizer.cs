﻿namespace ExactOnline.Client.OAuth2.WinForms;

public class ExactOnlineWinFormsAuthorizer : ExactOnlineAuthorizer
{
	public ExactOnlineWinFormsAuthorizer(string clientId, string clientSecret, Uri callbackUrl, string baseUrl = "https://start.exactonline.be", string? accessToken = null, string? refreshToken = null, DateTime? expiresAt = null)
		: base(clientId, clientSecret, callbackUrl, baseUrl, accessToken, refreshToken, expiresAt)
	{
	}

	public override async Task<string?> GetAccessTokenAsync(CancellationToken ct)
	{
		if (await IsAuthorizationNeededAsync(ct))
		{
			var authorizationUri = await GetLoginLinkUriAsync(ct: ct);

			var loginDialog = new LoginForm(new(authorizationUri), new(Configuration.RedirectUri));

			loginDialog.ShowDialog();

			if (loginDialog.AuthorizationCode is string code && !string.IsNullOrWhiteSpace(code))
			{
				await ProcessAuthorizationAsync(code, ct);
			}
		}

		return await base.GetAccessTokenAsync(ct);
	}
}
