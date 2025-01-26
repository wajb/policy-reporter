namespace PolicyReporter.DataHandling;

/// <inheritdoc cref="IPolicyReader"/>
/// <param name="destination">Repository in which to store normalised policies.</param>
public class PolicyReader(IPolicyRepository destination) : IPolicyReader
{
    private readonly List<(IPolicySource Source, IPolicyParserStrategy Strategy)> _readInstructions = new();

    /// <inheritdoc/>
    public IPolicyReader Read(IPolicySource source, IPolicyParserStrategy strategy)
    {
        _readInstructions.Add((source, strategy));

        return this;
    }

    /// <inheritdoc/>
    public async Task<ReadResults> Execute()
    {
        Func<Task>? flushUpserts = null;
        int policyCount = 0;

        foreach ((IPolicySource source, IPolicyParserStrategy strategy) in _readInstructions)
        {
            await foreach (Policy policy in strategy.ParsePolicies(source))
            {
                flushUpserts = await destination.Upsert(policy);
                policyCount++;
            }
        }

        if (flushUpserts is not null)
            await flushUpserts.Invoke();

        return new ReadResults(SourceCount: _readInstructions.Count, policyCount);
    }
}
