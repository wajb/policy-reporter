using Microsoft.EntityFrameworkCore;

namespace PolicyReporter.DataHandling.Models;

/// <summary>
/// A normalized policy.
/// </summary>
[PrimaryKey(nameof(PolicyId), nameof(SourceId))]
public record Policy
{
    /// <summary>
    /// Identifier of policy according to its source broker.
    /// </summary>
    required public string PolicyId { get; init; }

    /// <summary>
    /// Identifier of source broker.
    /// </summary>
    required public int SourceId { get; init; }

    /// <summary>
    /// ISO three-digit currency code.
    /// </summary>
    required public string Currency { get; init; }

    /// <summary>
    /// Insured amount for policy, in units of <see cref="Currency"/>.
    /// </summary>
    required public int InsuredAmount { get; init; }

    /// <summary>
    /// Identifier of customer.
    /// </summary>
    required public string Customer { get; init; }

    /// <summary>
    /// Policy start date.
    /// </summary>
    public DateOnly? StartDate { get; init; }

    /// <summary>
    /// Policy end date.
    /// </summary>
    public DateOnly? EndDate { get; init; }

    // TODO map further fields as required
}
