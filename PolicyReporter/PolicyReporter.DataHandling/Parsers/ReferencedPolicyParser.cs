namespace PolicyReporter.DataHandling.Parsers;

/// <inheritdoc/>
public class ReferencedPolicyParser(ICsvReaderFactory csvReaderFactory)
    : CsvPolicyParser<SourceReferencedPolicy>(csvReaderFactory)
{
    /// <inheritdoc/>
    protected override Policy MapToPolicy(SourceReferencedPolicy record, int sourceId) => new()
    {
        SourceId = sourceId,
        PolicyId = record.PolicyRef,
        InsuredAmount = record.CoverageAmount,
        Customer = record.CompanyDescription,
        StartDate = record.InitiationDate,
        EndDate = record.ExpirationDate,
        Currency = "",
    };
}
