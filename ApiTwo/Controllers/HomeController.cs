using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ApiTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }    
        
        public async Task<IActionResult> Index()
        {
            //retrieve access token 
            var serverClient = _httpClientFactory.CreateClient();

            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44336/");

            var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest 
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "client_id",
                    ClientSecret = "client_secret",
                    Scope = "scope_one:read" //what resource we're trying to access, we have to make sure it's registered in the server
                });


            //retrieve secret data

            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:44339/secret");
            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                secret_message = content

            });
        }
    }
}
