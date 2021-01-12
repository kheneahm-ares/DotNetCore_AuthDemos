using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
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
    }
}