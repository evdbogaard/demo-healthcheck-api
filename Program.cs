var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddApplicationInsightsTelemetry()
    .AddSingleton<HealthService>();

builder.Services
    .AddHealthChecks();

#region health checks
builder.Services
    .AddHealthChecks()
    .AddCheck<CustomCheck>(nameof(CustomCheck))
    .AddExternalApiCheck("My first external api", "https://demo-healthceck-external-api.azurewebsites.net/", tags: new[] { "api" })
    .AddExternalApiCheck("My second external api", "https://demo-healthceck-external-api-2.azurewebsites.net/", tags: new[] { "api" })
    .AddCosmosDb(builder.Configuration["CosmosDbConnString"], "master", tags: new[] { "db" })
    .AddApplicationInsightsPublisher();
#endregion

#region custom publisher
builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Period = TimeSpan.FromMinutes(5);
    options.Delay = TimeSpan.FromSeconds(15);
});
builder.Services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
#endregion

#region health check UI
builder.Services
    .AddHealthChecksUI(options =>
    {
        options.AddHealthCheckEndpoint("Healthcheck API", "/hc-json");
        options.AddHealthCheckEndpoint("Startup", "/hc-startup");
    })
    .AddInMemoryStorage();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage()
        .UseSwagger()
        .UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/hc-basic");

#region health check json
app.MapHealthChecks("/hc-json", new()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/hc-startup", new()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    Predicate = hc => hc.Tags.Contains("db") || hc.Tags.Contains("api")
});
#endregion

#region health check UI
app.MapHealthChecksUI(options => options.UIPath = "/hc-ui");
#endregion

app.Run();