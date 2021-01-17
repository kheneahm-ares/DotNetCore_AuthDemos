using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
           _authorizationService = authorizationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
        public IActionResult Authenticate()
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "Some_Id"), //the id
                new Claim("MyCustomClaim", "CoolClaim"), 
            };


            //create key and algorithm
            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            //use key and algo to create signing credentials
            var singingCredentials = new SigningCredentials(key, algorithm);

            //create token
            var token = new JwtSecurityToken(Constants.Issuer, 
                                             Constants.Audience, 
                                             claims, 
                                             notBefore: DateTime.Now, 
                                             expires: DateTime.Now.AddMinutes(60), 
                                             signingCredentials: singingCredentials);

            //turn token into string using handler
            var jsonToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new {access_token = jsonToken });
        }
    }
}