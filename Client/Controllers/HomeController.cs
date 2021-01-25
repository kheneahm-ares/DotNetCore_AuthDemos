using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
           _client = httpClientFactory.CreateClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await _client.GetAsync("https://localhost:44355/secret/index");

            var apiResponse = await _client.GetAsync("https://localhost:44376/secret/index");

            return View();
        }


        public async Task<string> RefreshToken()
        {

            var refresh_token = await HttpContext.GetTokenAsync("refresh_token");

            _client.DefaultRequestHeaders.Clear();

            var basicCreds = "someUsername:somePass";
            var encodedCreds = Encoding.UTF8.GetBytes(basicCreds);
            var base64Creds = Convert.ToBase64String(encodedCreds);

            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Creds}");

            var data = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refresh_token
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44355/oauth/token");
            request.Content = new FormUrlEncodedContent(data);

            var response = await _client.SendAsync(request);


            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);

            var access_token = responseData["access_token"];
            var new_refresh_token = responseData["refresh_token"]; //potenatially new

            //grab cookie using cookie scheme
            var authInfo = await HttpContext.AuthenticateAsync("ClientCookie");

            //grab properties (not claims) and update our tokens
            authInfo.Properties.UpdateTokenValue("access_token", access_token);
            authInfo.Properties.UpdateTokenValue("refresh_token", refresh_token);

            //sign us back in but with the same principal but updated properties
            await HttpContext.SignInAsync("ClientCookie", authInfo.Principal, authInfo.Properties);

            return await response.Content.ReadAsStringAsync();

        }

    }
}
