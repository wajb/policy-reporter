namespace PolicyReporter.DataHandling.Parsers;

/// <inheritdoc/>
public class NumberedPolicyParser(ICsvReaderFactory csvReaderFactory)
    : CsvPolicyParser<SourceNumberedPolicy>(csvReaderFactory)
{
    /// <inheritdoc/>
    protected override Policy MapToPolicy(SourceNumberedPolicy record, int sourceId) => new()
    {
        SourceId = sourceId,
        PolicyId = record.PolicyNumber,
        InsuredAmount = record.InsuredAmount,
        Customer = record.BusinessDescription,
        StartDate = record.StartDate,
        EndDate = record.EndDate,
        Currency = "",
    };
}
