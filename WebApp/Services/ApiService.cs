using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChatChainCommon.Config;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WebApp.Api;

namespace WebApp.Services
{
    public class ApiService
    {
        private readonly IdentityServerConnection _identityServerConnection;
        private readonly ApiConnection _apiConnection;

        public ApiService(IdentityServerConnection identityServerConnection, ApiConnection apiConnection)
        {
            _identityServerConnection = identityServerConnection;
            _apiConnection = apiConnection;
        }

        public async Task<ApiClient> GetApiClientAsync(HttpContext httpContext)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await httpContext.GetTokenAsync("access_token"));
            return new ApiClient(_apiConnection.ServerUrl, httpClient);
        }

        public async Task<bool> VerifyTokensAsync(HttpContext httpContext)
        {
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(await httpContext.GetTokenAsync("access_token"));
            DateTime now = DateTime.Now.ToUniversalTime();

            TimeSpan timeRemaining = jwtSecurityToken.ValidTo.Subtract(now);

            if (TimeSpan.Zero >= timeRemaining)
            {
                TokenResponse response = await new HttpClient().RequestRefreshTokenAsync(
                    new RefreshTokenRequest
                    {
                        Address = $"{_identityServerConnection.ServerUrl}/connect/token",
                        ClientId = _identityServerConnection.ClientId,
                        ClientSecret = _identityServerConnection.ClientSecret,
                        RefreshToken = await httpContext.GetTokenAsync("refresh_token")
                    });
                
                if (!response.IsError)
                {
                    AuthenticateResult auth = await httpContext.AuthenticateAsync("Cookies");
                    auth.Properties.StoreTokens(new List<AuthenticationToken>
                    {
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.AccessToken,
                            Value = response.AccessToken
                        },
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.RefreshToken,
                            Value = response.RefreshToken
                        }
                    });

                    await httpContext.SignInAsync(auth.Principal, auth.Properties);
                }

                if (response.IsError)
                {
                    return false;
                }
            }

            return true;
        }
    }
}