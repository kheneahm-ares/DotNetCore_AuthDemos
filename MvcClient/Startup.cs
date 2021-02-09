using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                config.DefaultScheme = "MyCookie";
                config.DefaultChallengeScheme = "myoidc";
            })
                .AddCookie("MyCookie")
                .AddOpenIdConnect("myoidc", config =>
                {
                    config.Authority = "https://localhost:44336/";

                    config.ClientId = "client_id_mvc";
                    config.ClientSecret = "client_secret_mvc";

                    config.SaveTokens = true;

                    config.ResponseType = "code"; // when we get challenged, we will tell the auth endpoint that we're using auth code flow

                    config.SignedOutCallbackPath = "/Home/Index"; //where we go to after logging out
                    //configure mappings from user info endpoints response to expected claims
                    config.ClaimActions.MapUniqueJsonKey("MappedTech.read", "tech.read");

                    //after we get id token, it will do ANOTHER roundtrip to the user endpoint
                    config.GetClaimsFromUserInfoEndpoint = true;

                    //scopes
                    config.Scope.Add("tech.scope");
                    config.Scope.Add("scope_one:read");
                    config.Scope.Add("offline_access");

                });

            services.AddHttpClient();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
