namespace Demo.HealthCheck.Api.HealthChecks;

public class ExternalApiCheck : IHealthCheck
{
    readonly HttpClient _client;
    readonly string? _url;

    public ExternalApiCheck(string? url)
    {
        _client = new HttpClient();
        _url = url;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = await _client.GetAsync(_url);
        return result.StatusCode == HttpStatusCode.OK
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy();
    }
}

public static class ExternalApiCheckBuilderExtensions
{
    const string DefaultName = "External Api Check";

    public static IHealthChecksBuilder AddExternalApiCheck(
        this IHealthChecksBuilder builder,
        string? name = default,
        string? url = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default)
    {
        return builder.Add(new HealthCheckRegistration(
            name ?? DefaultName,
            sp => new ExternalApiCheck(url),
            failureStatus,
            tags));
    }
}