using Microsoft.Extensions.DependencyInjection;
using PolicyReporter.DataHandling;
using PolicyReporter.DataHandling.Extensions;
using PolicyReporter.DataHandling.Models;
using PolicyReporter.DataHandling.Parsers;

Console.WriteLine("Policy aggregation starting...");

IServiceProvider services = new ServiceCollection()
    .AddPolicyHandling()
    .AddPolicyDbContext()
    .BuildServiceProvider();

var policyReader = services.GetRequiredService<IPolicyReader>();

ReadResults readResults = await policyReader
    .Read(new FilePolicySource("./broker1.csv", sourceId: 1), services.GetRequiredService<NumberedPolicyParser>())
    .Read(new FilePolicySource("./broker2.csv", sourceId: 2), services.GetRequiredService<ReferencedPolicyParser>())
    .Execute();

Console.WriteLine($"Done aggregating {readResults.PolicyCount} policies from {readResults.SourceCount} sources.");
