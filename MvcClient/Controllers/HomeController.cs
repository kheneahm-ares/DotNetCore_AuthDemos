using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            await RefreshAccessToken();

            var claims = User.Claims;

            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

            var secretMessage = await GetAPIOneSecret(accessToken);

            return View();
        }

        private async Task RefreshAccessToken()
        {
            var serverClient = _httpClientFactory.CreateClient();
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            //send refresh token to server's token endpoint
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44336/");
            var tokenEndpoint = discoveryDocument.TokenEndpoint;


            //letting identity server abstract all the work for us and use its objects
            var tokenResponse = await serverClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                RefreshToken = refreshToken,
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "client_id_mvc",
                ClientSecret = "client_secret_mvc"
            });

            //grab cookie and repopulate refresh and access
            var authInfo = await HttpContext.AuthenticateAsync("MyCookie");

            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            await HttpContext.SignInAsync("MyCookie", authInfo.Principal, authInfo.Properties);

        }

        private async Task<string> GetAPIOneSecret(string accessToken)
        {

            //retrieve secret data
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.GetAsync("https://localhost:44339/secret");
            var content = await response.Content.ReadAsStringAsync();

            return content;

        }
        
    }
}
