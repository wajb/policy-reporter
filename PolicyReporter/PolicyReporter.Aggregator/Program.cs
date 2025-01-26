using PolicyReporter.DataHandling;
using PolicyReporter.DataHandling.Models;
using PolicyReporter.DataHandling.Parsers;

Console.WriteLine("Policy aggregation starting...");

PolicyReader policyReader = new(new PolicyRepository(new PolicyDbContext()));

ReadResults readResults = await policyReader
    .Read(new FilePolicySource("./broker1.csv", sourceId: 1), new NumberedPolicyParser())
    .Read(new FilePolicySource("./broker2.csv", sourceId: 2), new ReferencedPolicyParser())
    .Execute();

Console.WriteLine($"Done aggregating {readResults.PolicyCount} policies from {readResults.SourceCount} sources.");
