using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PolicyReporter.DataHandling.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services for policy data handling.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection instance.</returns>
    public static IServiceCollection AddPolicyHandling(this IServiceCollection services) => services
        .AddScoped<IPolicyReader, PolicyReader>()
        .AddScoped<IPolicyRepository, PolicyRepository>()
        .AddScoped<NumberedPolicyParser>()
        .AddScoped<ReferencedPolicyParser>()
        .AddSingleton<ICsvReaderFactory, CsvReaderFactory>();

    /// <summary>
    /// Adds a default database context.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection instance.</returns>
    public static IServiceCollection AddPolicyDbContext(this IServiceCollection services) => services
        .AddDbContext<PolicyDbContext>(options =>
            options.UseSqlite($"Data Source={Path.Join(PathUtils.GetRootPath(), "policies.db")}"));
}
