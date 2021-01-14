using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics.Controllers
{
    public class OperationsController : Controller
    {
        private readonly IAuthorizationService _authService;

        public OperationsController(IAuthorizationService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Open()
        {
            //create new requirement that will get handled by the handler which is injected by the middleware
            var cookieJar = new CookieJar
            {
                Name = "SomeName"
            }; //this resource could be retrieve from the DB

            //what operation the user is trying to perform on the resource
            var requirement = new OperationAuthorizationRequirement()
            {
                Name = CookieJarOperations.Open
            };

            var result = await  _authService.AuthorizeAsync(User, cookieJar, requirement);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> AuthenticateAsFred()
        {
            //create claims
            var friendClaims = new List<Claim>
            {
                new Claim("Friend", "Fred")
            };

            //create identity
            var friendAssociation = new ClaimsIdentity(friendClaims, "Association of Friends Identity");

            //create principal
            var userPrinicipal = new ClaimsPrincipal(friendAssociation);

            //signs in and effectively ends up as a cookie
            await HttpContext.SignInAsync(userPrinicipal);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AuthenticateAsBob()
        {
            //create claims
            var friendClaims = new List<Claim>
            {
                new Claim("Friend", "Bob")
            };

            //create identity
            var friendAssociation = new ClaimsIdentity(friendClaims, "Association of Friends Identity");

            //create principal
            var userPrinicipal = new ClaimsPrincipal(friendAssociation);

            //signs in and effectively ends up as a cookie
            await HttpContext.SignInAsync(userPrinicipal);

            return RedirectToAction("Index");
        }
    }

    public class CookieJarAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, CookieJar>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, CookieJar cookieJar)
        {

            if(requirement.Name == CookieJarOperations.Look)
            {
                if(context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }    
            //only our friend Bob can open our cookie jar
            else if (requirement.Name == CookieJarOperations.Open)
            {
                if(context.User.Identity.IsAuthenticated && context.User.HasClaim("Friend", "Bob"))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;

        }
    }

    public static class CookieJarOperations
    {
        public static string Open = "Open";
        public static string TakeCookie = "TakeCookie";
        public static string ComeNear = "ComeNear";
        public static string Look = "Look";

    }

    /// <summary>
    /// The Resource we might want to access
    /// </summary>
    public class CookieJar
    {
        public string Name { get; set; }
    }

}
