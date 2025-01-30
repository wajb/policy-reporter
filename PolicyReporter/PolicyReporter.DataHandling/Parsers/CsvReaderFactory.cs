using CsvHelper;
using CsvHelper.Configuration;

namespace PolicyReporter.DataHandling.Parsers;

/// <inheritdoc cref="ICsvReaderFactory"/>
public class CsvReaderFactory : ICsvReaderFactory
{
    /// <inheritdoc/>
    public CsvReader CreateCsvReader(TextReader reader, IReaderConfiguration config) => new(reader, config);
}
