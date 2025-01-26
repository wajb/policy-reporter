using PolicyReporter.DataHandling.Models;

namespace PolicyReporter.Server.Models;

/// <summary>
/// Object describing a policy report.
/// </summary>
public record Report
{
    required public PolicyStatistics Statistics { get; init; }

    required public List<Policy> Policies { get; init; }
}
