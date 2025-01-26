using Microsoft.EntityFrameworkCore;

namespace PolicyReporter.DataHandling;

/// <summary>
/// Database session for policy records.
/// </summary>
public class PolicyDbContext() : DbContext
{
    private static readonly string _databasePath = Path.Join(PathUtils.GetRootPath(), "policies.db");

    /// <summary>
    /// Policy records.
    /// </summary>
    public DbSet<Policy> Policies { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_databasePath}");
}
