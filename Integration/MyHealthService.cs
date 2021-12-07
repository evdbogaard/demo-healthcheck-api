namespace Demo.HealthCheck.Api.Integration
{
    public class MyHealthService
    {
        public bool Healthy { get; set; } = true;
        public int Status { get; set; }
    }
}