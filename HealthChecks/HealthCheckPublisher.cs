namespace Demo.HealthCheck.Api.HealthChecks;

public class HealthCheckPublisher : IHealthCheckPublisher
{
    private readonly TelemetryClient _tc;

    public HealthCheckPublisher(TelemetryClient tc) => _tc = tc;

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        var entriesDict = report.Entries.ToDictionary(e => e.Key, e => $"{e.Value.Status} => {e.Value.Description}");
        entriesDict.Add("Global", report.Status.ToString());

        _tc.TrackEvent("health-check", entriesDict);

        cancellationToken.ThrowIfCancellationRequested();

        return Task.CompletedTask;
    }
}