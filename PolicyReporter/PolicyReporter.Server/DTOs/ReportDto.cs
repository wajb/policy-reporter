namespace PolicyReporter.Server.DTOs;

/// <summary>
/// Contract object describing a policy report.
/// </summary>
public record ReportDto
{
    required public Models.PolicyStatistics Statistics { get; init; }

    required public List<PolicyDto> Policies { get; init; }
}
