using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                // we check the cookie to confirm that we are authenticated
                config.DefaultAuthenticateScheme = "ClientCookie";

                // when we sign in, we will create a cookie
                config.DefaultSignInScheme = "ClientCookie";

                // where we check if we are allowed to do something/authenticate/challenge the cookie
                config.DefaultChallengeScheme = "OurServer";

            })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer", config =>
                {
                    config.ClientId = "some_client_id";
                    config.ClientSecret = "some_client_secret";
                    config.CallbackPath = "/oauth/callback";
                    config.AuthorizationEndpoint = "https://localhost:44355/oauth/authorize"; //where the browser will redirect to authorize
                    config.TokenEndpoint = "https://localhost:44355/oauth/token"; //where the client will call to get an access token

                    config.SaveTokens = true; //purposely save token

                    config.Events = new OAuthEvents()
                    {
                        //when we receive a token from the server
                        //populate the claims in the HttpContext for the user who authenticated
                        OnCreatingTicket = context =>
                        {

                            var accessToken = context.AccessToken;
                            var payload = accessToken.Split('.')[1];


                            var bytes = Convert.FromBase64String(payload);
                            var jsonPayload = Encoding.UTF8.GetString(bytes);
                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                            //foreach claim from the token, populate the claims in the HttpContext
                            foreach (var claim in claims)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });


            services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //who are you
            app.UseAuthentication();

            //are you allowed?
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
