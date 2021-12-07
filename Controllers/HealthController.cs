using Demo.HealthCheck.Api.Integration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Demo.HealthCheck.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        readonly MyHealthService _healthService;

        public HealthController(MyHealthService healthService, IConfiguration configuration) => _healthService = healthService;

        [HttpGet("/")]
        public IActionResult MainGet() => Ok("Hello world");

        [HttpGet]
        public void Get() => _healthService.Healthy = !_healthService.Healthy;

        [HttpGet("SetHealthy")]
        public void SetHealthy() => _healthService.Status = 0;

        [HttpGet("SetDegraded")]
        public void SetDegraded() => _healthService.Status = 1;

        [HttpGet("SetUnhealthy")]
        public void SetUnhealthy() => _healthService.Status = 2;
    }
}