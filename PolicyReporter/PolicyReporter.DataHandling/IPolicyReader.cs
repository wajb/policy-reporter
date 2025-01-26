namespace PolicyReporter.DataHandling;

/// <summary>
/// Service to orchestrate aggregating policies from multiple sources.
/// </summary>
public interface IPolicyReader
{
    /// <summary>
    /// Defines a source of policy data and a strategy for parsing it. Invoke <see cref="Execute"/> to initiate
    /// aggregration.
    /// </summary>
    /// <param name="source">Source from which to read policy data.</param>
    /// <param name="strategy">Strategy for parsing data from the given <paramref name="source"/>.</param>
    IPolicyReader Read(IPolicySource source, IPolicyParserStrategy strategy);

    /// <summary>
    /// Initiate aggregation of policies.
    /// </summary>
    /// <returns>Task that completes with a summary of the operation.</returns>
    Task<ReadResults> Execute();
}
