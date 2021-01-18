using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

            const string code = "SomeCODE";


            return Redirect($"{redirect_uri}");

        }

        [HttpGet]
        public IActionResult Token()
        {
            return View();

        }
    }
}
