namespace PolicyReporter.Server.DTOs;

/// <summary>
/// Contract object describing statistics for a set of policies.
/// </summary>
public record PolicyStatisticsDto
{
    public int PolicyCount { get; init; }

    public int CustomerCount { get; init; }

    public int InsuredAmountTotal { get; init; }

    public int AveragePolicyDurationDays { get; init; }
}
