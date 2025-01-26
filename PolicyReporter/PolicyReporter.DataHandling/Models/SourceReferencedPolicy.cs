using System.Globalization;
using CsvHelper.Configuration.Attributes;

namespace PolicyReporter.DataHandling.Models;

/// <summary>
/// A policy from a source that describes policies by references, formatted with <c>en-GB</c> regional date formatting and
/// currency.
/// </summary>
[CultureInfo("en-GB")]
public record SourceReferencedPolicy
{
    required public string PolicyRef { get; init; }

    required public string CompanyDescription { get; init; }

    public int CoverageAmount { get; init; }

    [TypeConverter(typeof(PossibleDateConverter))]
    public DateOnly? InitiationDate { get; init; }

    [TypeConverter(typeof(PossibleDateConverter))]
    public DateOnly? ExpirationDate { get; init; }
}
