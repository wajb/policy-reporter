using System.Globalization;
using CsvHelper.Configuration.Attributes;

namespace PolicyReporter.DataHandling.Models;

/// <summary>
/// A policy from a source that describes policies by numbers, formatted with <c>en-GB</c> regional date formatting and
/// currency.
/// </summary>
[CultureInfo("en-GB")]
public record SourceNumberedPolicy
{
    required public string PolicyNumber { get; init; }

    required public string BusinessDescription { get; init; }

    public int InsuredAmount { get; init; }

    [TypeConverter(typeof(PossibleDateConverter))]
    public DateOnly? StartDate { get; init; }

    [TypeConverter(typeof(PossibleDateConverter))]
    public DateOnly? EndDate { get; init; }
}
