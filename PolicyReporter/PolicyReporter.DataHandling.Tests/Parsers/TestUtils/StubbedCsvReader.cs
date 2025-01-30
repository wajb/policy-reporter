using System.Globalization;
using CsvHelper;

namespace PolicyReporter.DataHandling.Tests.Parsers.TestUtils;

/// <summary>
/// A version of <see cref="CsvReader"/> whose results can be stubbed for testing.
/// </summary>
/// <typeparam name="TRecord">Type of record that is read.</typeparam>
/// <param name="reader">Source reader.</param>
/// <param name="records">Stubbed records.</param>
public class StubbedCsvReader<TRecord>(TextReader reader)
    : CsvReader(reader, CultureInfo.InvariantCulture)
{
    public IAsyncEnumerable<TRecord> Records { get; set; } = new List<TRecord>().ToAsyncEnumerable();

    public override IAsyncEnumerable<T> GetRecordsAsync<T>(CancellationToken cancellationToken = default) =>
        (IAsyncEnumerable<T>)Records;
}
