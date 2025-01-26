namespace PolicyReporter.DataHandling.Models;

/// <summary>
/// Object representing results of a policy aggregation.
/// </summary>
/// <param name="SourceCount">Count of sources.</param>
/// <param name="PolicyCount">Count of policies processed.</param>
public record ReadResults(int SourceCount, int PolicyCount);
