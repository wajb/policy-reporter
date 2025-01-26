using PolicyReporter.Server.Models;

namespace PolicyReporter.Server.Services;

/// <summary>
/// Service for generating policy reports.
/// </summary>
public interface IPolicyReportService
{
    /// <summary>
    /// Gets a report of all policies, optionally filtered by broker.
    /// </summary>
    /// <returns>Task that resolves a policy report.</returns>
    Task<Report> GetPolicies(string? brokerNameOrId = null);

    /// <summary>
    /// Gets a report of active policies (those started and not completed), optionally filtered by broker.
    /// </summary>
    /// <returns>Task that resolves a policy report.</returns>
    Task<Report> GetActivePolicies(string? brokerNameOrId = null);
}
