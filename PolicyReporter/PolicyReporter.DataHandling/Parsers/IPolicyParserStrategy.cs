namespace PolicyReporter.DataHandling.Parsers;

/// <summary>
/// Strategy for parsing policies for aggregation.
/// </summary>
public interface IPolicyParserStrategy
{
    /// <summary>
    /// Parse polices from a given source asynchronously.
    /// </summary>
    /// <param name="source">Source from which to access policy data.</param>
    /// <returns>Enumerator of resultant policies.</returns>
    /// <exception cref="PolicyParseException">Thrown if parsing fails.</exception>
    IAsyncEnumerable<Policy> ParsePolicies(IPolicySource source);
}
