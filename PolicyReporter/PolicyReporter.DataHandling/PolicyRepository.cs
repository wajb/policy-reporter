using Microsoft.EntityFrameworkCore;

namespace PolicyReporter.DataHandling;

/// <inheritdoc cref="IPolicyRepository"/>
public class PolicyRepository(PolicyDbContext dbContext) : IPolicyRepository
{
    private readonly PolicyDbContext _dbContext = dbContext;

    private void InitDatabase() => _dbContext.Database.Migrate();

    /// <inheritdoc/>
    public Task<Func<Task>> Upsert(Policy policy)
    {
        InitDatabase();

        if (_dbContext.Policies.Any(p => p.PolicyId == policy.PolicyId && policy.SourceId == policy.SourceId))
        {
            Console.WriteLine($"Updating {policy.PolicyId} ({policy.SourceId})");
            _dbContext.Update(policy);
        }
        else
        {
            Console.WriteLine($"Inserting {policy.PolicyId} ({policy.SourceId})");
            _dbContext.Add(policy);
        }

        // TODO add batched saves if necessary

        return Task.FromResult<Func<Task>>(() => _dbContext.SaveChangesAsync());
    }

    /// <inheritdoc/>
    public Task<List<Policy>> GetAllPolicies() =>
        _dbContext.Policies.AsNoTracking().ToListAsync();

    /// <inheritdoc/>
    public Task<List<Policy>> GetPoliciesBySourceId(int sourceId) =>
        _dbContext.Policies.AsNoTracking().Where(p => p.SourceId == sourceId).ToListAsync();
}
