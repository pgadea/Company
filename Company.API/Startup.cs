using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Company.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Company.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "companyApi";
                });

            services.AddDbContext<CompanyContext>(options => 
                options.UseInMemoryDatabase("CompanyDb"));

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Company API", Version = "v1" });
                options.OperationFilter<CheckAuthorizeOperationFilter>();

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = "http://localhost:5000/connect/authorize",
                    TokenUrl = "http://localhost:5000/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "companyApi", "Customer API for Company" }
                    }
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Company API V1");
                options.OAuthClientId("swaggerapiui");
                options.OAuthAppName("Swagger API UI");
            });

            app.UseMvc();
        }

        internal class CheckAuthorizeOperationFilter : IOperationFilter
        {
            public void Apply(Operation operation, OperationFilterContext context)
            {
                var authorizeAttributeExists = context.ApiDescription.ControllerAttributes().OfType<AuthorizeAttribute>().Any()
                    || context.ApiDescription.ActionAttributes().OfType<AuthorizeAttribute>().Any();

                if (authorizeAttributeExists)
                {
                    operation.Responses.Add("401", new Response { Description = "Unauthorized" });
                    operation.Responses.Add("403", new Response { Description = "Forbidden" });

                    operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
                    operation.Security.Add(new Dictionary<string, IEnumerable<string>>
                    {
                        { "oauth2", new [] { "companyApi" } }
                    });
                }
            }
        }
    }
}
