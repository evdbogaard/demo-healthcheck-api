using System.Threading;
using System.Threading.Tasks;
using Demo.HealthCheck.Api.Integration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demo.HealthCheck.Api.HealthChecks
{
    public class CustomCheck : IHealthCheck
    {
        readonly MyHealthService _healthService;

        public CustomCheck(MyHealthService healthService) => _healthService = healthService;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            switch(_healthService.Status)
            {
                case 1:
                    return Task.FromResult(HealthCheckResult.Degraded());
                case 2:
                    return Task.FromResult(HealthCheckResult.Unhealthy());
                default:
                    return Task.FromResult(HealthCheckResult.Healthy());
            };
        }
    }
}