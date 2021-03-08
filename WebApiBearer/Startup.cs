using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace KeyCloak3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                options.Authority = this.Configuration["Oidc:Authority"];
                options.Audience = this.Configuration["Oidc:ClientId"];
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    //ValidAudiences = new[] { "master-realm", "account" },
                    ValidateIssuer = true,
                    ValidIssuer = this.Configuration["Oidc:Authority"],
                    ValidateLifetime = true
                };
                options.RequireHttpsMetadata = false;
            });

            //access_token = POST http://<YOUR_KEYCLOAK_HOST>:<YOUR_KEYCLOAK_PORT>/auth/realms/<YOUR_REALM>/protocol/openid-connect/token
            /*  BODY:
                client_id:<YOUR_CLIENT_ID>
                client_secret:<YOUR_CLIENT_SECRET>
                grant_type:password
                scope:openid
                username:<YOUR_USERNAME>
                password:<YOUR_PASSWORD>
            */

            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication(); // added
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
