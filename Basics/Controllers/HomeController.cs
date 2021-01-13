using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
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

        [Authorize("Claim.DoB")]
        public IActionResult SecretDoB()
        {
            return View("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }


        public IActionResult Authenticate()
        {
            //build our claimss
            var ourUniqueClaims = new List<Claim>()
            {
              new Claim(ClaimTypes.Name, "Bob"),
              new Claim(ClaimTypes.Email, "Bob@bob.com"),
              new Claim(ClaimTypes.DateOfBirth, "12/12/2021"),
              new Claim(ClaimTypes.Role, "Admin" ),
              new Claim("Some.Unique.Claim", "Bob Is Cool"),
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim("DrivingLicenseGrade", "A+++"),
                new Claim(ClaimTypes.Name, "Bob Johnson")
            };

            //create identity based on claims
            var userIdentity = new ClaimsIdentity(ourUniqueClaims, "Kheneahm's Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government License Identity");

            //a principal can consists of different identies bc many "AUTHORITIES" like google or facebook have different "claims" about you
            var userPrincipal = new ClaimsPrincipal(new[] { userIdentity, licenseIdentity });

            //signs us in
            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff()
        {
            //we are doing some stuff here


            //after we try to authorize given an existing policy
           //await _authorizationService.AuthorizeAsync(HttpContext.User, "Claim.DoB");


            //we can also create a custom policy
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, customPolicy);

            if(authResult.Succeeded)
            {

            }



            return View("Index");
        }
    }
}