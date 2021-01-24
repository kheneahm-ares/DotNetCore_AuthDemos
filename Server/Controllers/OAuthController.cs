using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class OAuthController : Controller
    {

        [HttpGet]
        public IActionResult Authorize(
            string response_type, // authorization flow type
            string client_id, //who the client is
            string redirect_uri, //where we redirect after authorizing
            string scope, // what information I want; ex: email, telephone
            string state) // randmon string generator to confirm that we are going back to the same client
        {
            var query = new QueryBuilder();
            query.Add("redirect_uri", redirect_uri);
            query.Add("state", state);

            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string username, 
            string redirect_uri,
            string state)
        {

            //verify username/pass


            //generate code, store in db

            const string code = "SomeCODE"; 

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);


            return Redirect($"{redirect_uri}{query.ToString()}");

        }

        public async Task<IActionResult> Token(
            string grant_type, //flow of access_token request
            string code, //confirmation of authentication process
            string redirect_uri,
            string client_id)
        {
            //some mechanism for validating the code
            //usually stored in the DB, code expires 



            //after validate, return access token
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
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token = access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            };

            var jsonObj = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(jsonObj);


            //easy way to write to a response
            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

            return Redirect(redirect_uri);

        }


        [Authorize]
        public IActionResult Validate()
        {
            if(HttpContext.Request.Query.TryGetValue("access_token", out var token))
            {
                //validate claims or something

                return Ok();
            }
            return BadRequest();
        }
    }
}
