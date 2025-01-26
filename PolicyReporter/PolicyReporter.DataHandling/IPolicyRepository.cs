namespace PolicyReporter.DataHandling;

/// <summary>
/// Repository for normalised policies.
/// </summary>
public interface IPolicyRepository
{
    /// <summary>
    /// Insert a policy, or update a policy if a match is already stored.
    /// </summary>
    /// <param name="policy">Policy record.</param>
    /// <returns>Flush action to commit all pending operations.</returns>
    Task<Func<Task>> Upsert(Policy policy);

    /// <summary>
    /// Gets all stored policies.
    /// </summary>
    /// <returns>List of <see cref="Policy"/>.</returns>
    Task<List<Policy>> GetAllPolicies();

    /// <summary>
    /// Gets all stored policies that match a given <see cref="Policy.SourceId"/>.
    /// </summary>
    /// <param name="sourceId">Identifier of source.</param>
    /// <returns>List of <see cref="Policy"/>.</returns>
    Task<List<Policy>> GetPoliciesBySourceId(int sourceId);
}
