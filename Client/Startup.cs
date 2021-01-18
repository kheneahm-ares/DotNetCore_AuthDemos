using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .AddCookie("ClienCookie")
                .AddOAuth("OurServer", config =>
                {
                    config.ClientId = "some_client_id";
                    config.ClientSecret = "some_client_secret";
                    config.CallbackPath = "/oauth/callback";
                    config.AuthorizationEndpoint = "https://localhost:44355/oauth/authorize"; //where the browser will redirect to authorize
                    config.TokenEndpoint = "https://localhost:44355/oauth/token"; //where the client will call to get an access token

                });


            services.AddControllersWithViews();

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
