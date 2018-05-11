using System.IO;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace Auth.Server
{
    public sealed class AuthorizationProvider : OpenIdConnectServerProvider
    {
        // Implement OnValidateAuthorizationRequest to support interactive flows (code/implicit/hybrid).
        public override async Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context) { }
        // Implement OnValidateTokenRequest to support flows using the token endpoint
        // (code/refresh token/password/client credentials/custom grant).
        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context) { }

        public override Task ExtractAuthorizationRequest(ExtractAuthorizationRequestContext context)
        {
            // If a request_id parameter can be found in the authorization request,
            // restore the complete authorization request stored in the user session.
            if (!string.IsNullOrEmpty(context.Request.RequestId))
            {
                var payload = context.HttpContext.Session.Get(context.Request.RequestId);
                if (payload == null)
                {
                    context.Reject(error: OpenIdConnectConstants.Errors.InvalidRequest, description: "Invalid request: timeout expired.");
                    return Task.FromResult(0);
                }
                // Restore the authorization request parameters from the serialized payload.
                using (var reader = new BsonReader(new MemoryStream(payload)))
                {
                    foreach (var parameter in JObject.Load(reader))
                    {
                        // Avoid overriding the current request parameters.
                        if (context.Request.HasParameter(parameter.Key))
                        {
                            continue;
                        }
                        context.Request.SetParameter(parameter.Key, parameter.Value);
                    }
                }
            }
            return Task.FromResult(0);
        }
    }

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        //// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddAuthentication();
        //    services.AddIdentity<ApplicationUser, IdentityRole>()
        //     .AddEntityFrameworkStores<ApplicationDbContext>();
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseOAuthValidation();

            app.UseOpenIdConnectServer(options =>
            {
                options.Provider = new AuthorizationProvider();
                options.AuthorizationEndpointPath = "/connect/authorize";
                options.AllowInsecureHttp = true;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
