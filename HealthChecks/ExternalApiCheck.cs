using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demo.HealthCheck.Api.HealthChecks
{
    public class ExternalApiCheck : IHealthCheck
    {
        readonly HttpClient _client;
        readonly string _url;

        public ExternalApiCheck(string url)
        {
            _client = new HttpClient();
            _url = url;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var result = await _client.GetAsync(_url);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return HealthCheckResult.Healthy();
            return HealthCheckResult.Unhealthy();
        }
    }

    public static class ExternalApiCheckBuilderExtensions
    {
        const string DefaultName = "External Api Check";

        public static IHealthChecksBuilder AddExternalApiCheck(
            this IHealthChecksBuilder builder,
            string name = default,
            string url = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default)
        {
            return builder.Add(new HealthCheckRegistration(
                name ?? DefaultName,
                sp => new ExternalApiCheck(url),
                failureStatus,
                tags));
        }
    }
}