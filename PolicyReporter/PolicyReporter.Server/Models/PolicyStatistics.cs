namespace PolicyReporter.Server.Models;

/// <summary>
/// Object describing statistics for a set of policies.
/// </summary>
public record PolicyStatistics
{
    public int PolicyCount { get; init; }

    public int CustomerCount { get; init; }

    public int InsuredAmountTotal { get; init; }

    public int AveragePolicyDurationDays { get; init; }
}
