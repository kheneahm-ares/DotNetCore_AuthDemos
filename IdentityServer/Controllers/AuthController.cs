using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            //validate view model


            var result = await _signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password, false, false);

            if(result.Succeeded)
            {
                return Redirect(viewModel.ReturnUrl);
            }
            
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Register(string returnUrl)
        {

            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            //validate view model
            if(!ModelState.IsValid)
            {
                return View(viewModel);
            }

            //create user
            var newUser = new IdentityUser(viewModel.Username);
            var result = await _userManager.CreateAsync(newUser, viewModel.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, false);

                return Redirect(viewModel.ReturnUrl);
            }

            return View(viewModel);
        }

    }
}
