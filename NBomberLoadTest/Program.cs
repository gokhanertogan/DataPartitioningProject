using NBomber.Contracts.Stats;
using NBomber.CSharp;

var httpClient = new HttpClient();

var scenario = Scenario.Create("http-load-test", async context =>
{
    var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5187/variant-campaigns");

    using var response = await httpClient.SendAsync(request);

    return response.IsSuccessStatusCode
        ? Response.Ok(sizeBytes: (int)response.Content.Headers.ContentLength!)
        : Response.Fail();
})
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 100, during: TimeSpan.FromSeconds(60))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportingInterval(TimeSpan.FromSeconds(10))
    .WithReportFolder("reports")
    .WithReportFormats(ReportFormat.Html)
    .Run();

Console.ReadKey();