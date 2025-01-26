namespace PolicyReporter.Server.DTOs;

/// <summary>
/// Contract object describing a policy.
/// </summary>
public record PolicyDto
{
    required public string PolicyId { get; init; }

    required public int SourceId { get; init; }

    required public string Currency { get; init; }

    required public int InsuredAmount { get; init; }

    required public string Customer { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }
}
