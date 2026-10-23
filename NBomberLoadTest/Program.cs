using NBomber.Contracts.Stats;
using NBomber.CSharp;

// Settings come from environment variables so the same project can target a local or containerized API.
var baseUrl = Environment.GetEnvironmentVariable("LOADTEST_BASE_URL") ?? "http://localhost:5187";
var copies = int.TryParse(Environment.GetEnvironmentVariable("LOADTEST_COPIES"), out var c) ? c : 100;
var durationSeconds = int.TryParse(Environment.GetEnvironmentVariable("LOADTEST_DURATION_SECONDS"), out var d) ? d : 60;
var warmUpSeconds = int.TryParse(Environment.GetEnvironmentVariable("LOADTEST_WARMUP_SECONDS"), out var w) ? w : 30;
var path = Environment.GetEnvironmentVariable("LOADTEST_PATH") ?? "/variant-campaigns";

using var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(60) };

var scenario = Scenario.Create("http-load-test", async context =>
{
    using var request = new HttpRequestMessage(HttpMethod.Get, path);
    using var response = await httpClient.SendAsync(request);

    // Read the whole body so the measured latency includes transferring the response.
    var body = await response.Content.ReadAsByteArrayAsync();

    return response.IsSuccessStatusCode
        ? Response.Ok(statusCode: ((int)response.StatusCode).ToString(), sizeBytes: body.Length)
        : Response.Fail(statusCode: ((int)response.StatusCode).ToString(), sizeBytes: body.Length);
})
.WithWarmUpDuration(TimeSpan.FromSeconds(warmUpSeconds))
.WithLoadSimulations(
    Simulation.KeepConstant(copies: copies, during: TimeSpan.FromSeconds(durationSeconds))
);

Console.WriteLine($"Target: {baseUrl}{path}, copies: {copies}, duration: {durationSeconds}s, warm-up: {warmUpSeconds}s");

NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportingInterval(TimeSpan.FromSeconds(10))
    .WithReportFolder("reports")
    .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
    .Run();
