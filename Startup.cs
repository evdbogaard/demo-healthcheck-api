using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Demo.HealthCheck.Api.Integration;
using Demo.HealthCheck.Api.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

namespace Demo.HealthCheck.Api
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo.HealthCheck.Api", Version = "v1" });
            });

            services
                .AddApplicationInsightsTelemetry(Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"])
                .AddSingleton<MyHealthService>();

            services
                .AddHealthChecks()
                 .AddCosmosDb(Configuration["CosmosDbConnString"], "master", "CosmosDb", tags: new[] { "startup" })
                 .AddCheck<CustomCheck>(nameof(CustomCheck))
                 .AddExternalApiCheck("My first external api", "https://demo-healthceck-external-api.azurewebsites.net/")
                 .AddExternalApiCheck("My second external api", "https://demo-healthceck-external-api-2.azurewebsites.net/");

            services
                .AddHealthChecksUI(options =>
                {
                    options.AddHealthCheckEndpoint("Healthcheck API", "/hc/live");
                })
                .AddInMemoryStorage();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo.HealthCheck.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/hc");

                endpoints.MapHealthChecks("/hc/live", new HealthCheckOptions
                {
                    Predicate = (check) => !check.Tags.Contains("startup"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecks("/hc/ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("startup")
                });

                endpoints.MapHealthChecksUI(options =>
                {
                    options.UIPath = "/hc-ui";
                });
            });
        }
    }
}
