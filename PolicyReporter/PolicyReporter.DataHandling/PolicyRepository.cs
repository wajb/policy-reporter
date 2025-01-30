using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace PolicyReporter.DataHandling;

/// <inheritdoc cref="IPolicyRepository"/>
public class PolicyRepository(PolicyDbContext dbContext) : IPolicyRepository
{
    private readonly PolicyDbContext _dbContext = dbContext;

    private void InitDatabase()
    {
        if (_dbContext.Database.IsRelational())
            _dbContext.Database.Migrate();
    }

    private void CheckTable()
    {
        try
        {
            _ = _dbContext.Policies.Count();
        }
        catch (SqliteException e) when (e.Message.Contains("no such table"))
        {
            throw new InvalidOperationException("Database does not exist - please run aggregator tool first!", e);
        }
    }

    /// <inheritdoc/>
    public Task<Func<Task>> Upsert(Policy policy)
    {
        InitDatabase();

        if (_dbContext.Policies.Any(p => p.PolicyId == policy.PolicyId && p.SourceId == policy.SourceId))
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
    public Task<List<Policy>> GetAllPolicies()
    {
        CheckTable();
        return _dbContext.Policies.AsNoTracking().ToListAsync();
    }

    /// <inheritdoc/>
    public Task<List<Policy>> GetPoliciesBySourceId(int sourceId)
    {
        CheckTable();
        return _dbContext.Policies.AsNoTracking().Where(p => p.SourceId == sourceId).ToListAsync();
    }
}
