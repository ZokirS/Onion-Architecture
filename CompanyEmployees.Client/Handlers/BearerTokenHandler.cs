using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using System.Globalization;

namespace CompanyEmployees.Client.Handlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        public BearerTokenHandler(IHttpContextAccessor contextAccessor, IHttpClientFactory clientFactory)
        {
            _contextAccessor = contextAccessor;
            _clientFactory = clientFactory;

        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            if(!string.IsNullOrEmpty(accessToken))
             request.SetBearerToken(accessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        public async Task<string> GetAccessToken()
        {
            var expiresAtToken = await _contextAccessor.HttpContext.GetTokenAsync("expires_at");
            var expiresDateTimeOffset = DateTimeOffset.Parse(expiresAtToken, CultureInfo.InvariantCulture);

            if ((expiresDateTimeOffset.AddSeconds(-60)).ToUniversalTime() > DateTime.UtcNow)
                return await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var refreshResponse = await GetRefreshResponseFromIDP();

            var updatedToken = GetUpdatedTokens(refreshResponse);

            var currentAuthenticateResult = await _contextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            currentAuthenticateResult.Properties.StoreTokens(updatedToken);

            await _contextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                currentAuthenticateResult.Principal,
                currentAuthenticateResult.Properties);

            return refreshResponse.AccessToken;
        }

        private async Task<TokenResponse> GetRefreshResponseFromIDP()
        {
            var idpClient = _clientFactory.CreateClient("IDPClient");
            var metaDataRespone = await idpClient.GetDiscoveryDocumentAsync();

            var refreshToken = await _contextAccessor
                .HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var refreshResponse = await idpClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest
                {
                    Address = metaDataRespone.TokenEndpoint,
                    ClientId = "companyemployeeclient",
                    ClientSecret = "CompanyEmployeeClientSecret",
                    RefreshToken = refreshToken
                });
            return refreshResponse;
        }

        private List<AuthenticationToken> GetUpdatedTokens(TokenResponse refreshResponse)
        {
            var updatedTokens = new List<AuthenticationToken>();
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.IdToken,
                Value = refreshResponse.IdentityToken
            });

            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = refreshResponse.AccessToken
            });

            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = refreshResponse.RefreshToken
            });

            updatedTokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = (DateTime.UtcNow + TimeSpan.FromSeconds(refreshResponse.ExpiresIn))
                .ToString("o", CultureInfo.InvariantCulture)
            });
            return updatedTokens;
        }
    }
}
