using System.Threading;
using System.Threading.Tasks;
using Demo.HealthCheck.Api.Integration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demo.HealthCheck.Api.HealthChecks
{
    public class CustomCheck : IHealthCheck
    {
        readonly HealthService _healthService;

        public CustomCheck(HealthService healthService) => _healthService = healthService;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            => _healthService.Healthy
                ? Task.FromResult(HealthCheckResult.Healthy())
                : Task.FromResult(HealthCheckResult.Unhealthy());
    }
}