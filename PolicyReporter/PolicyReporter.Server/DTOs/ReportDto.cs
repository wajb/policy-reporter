namespace PolicyReporter.Server.DTOs;

/// <summary>
/// Contract object describing a policy report.
/// </summary>
public record ReportDto
{
    required public PolicyStatisticsDto Statistics { get; init; }

    required public List<PolicyDto> Policies { get; init; }
}
