using Demo.HealthCheck.Api.Integration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Demo.HealthCheck.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        readonly HealthService _healthService;

        public HealthController(HealthService healthService, IConfiguration configuration) => _healthService = healthService;

        [HttpGet]
        public void Get() => _healthService.Healthy = !_healthService.Healthy;
    }
}