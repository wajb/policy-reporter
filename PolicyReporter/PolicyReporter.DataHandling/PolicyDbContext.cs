using Microsoft.EntityFrameworkCore;

namespace PolicyReporter.DataHandling;

/// <summary>
/// Database session for policy records.
/// </summary>
public class PolicyDbContext : DbContext
{
    /// <summary>
    /// Policy records.
    /// </summary>
    public DbSet<Policy> Policies { get; set; } = null!;

    /// <summary>
    /// Creates a <see cref="PolicyDbContext"/> instance.
    /// </summary>
    /// <param name="options">Context options.</param>
    public PolicyDbContext(DbContextOptions<PolicyDbContext> options) : base(options)
    {
    }
}
