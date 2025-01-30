using CsvHelper;
using CsvHelper.Configuration;

namespace PolicyReporter.DataHandling.Parsers;

/// <summary>
/// Factory that creates <see cref="CsvReader"/> instances.
/// </summary>
public interface ICsvReaderFactory
{
    /// <summary>
    /// Creates a <see cref="CsvReader"/> intance.
    /// </summary>
    /// <param name="reader">Text reader.</param>
    /// <param name="config">Configuration object.</param>
    /// <returns>CSV file reader.</returns>
    CsvReader CreateCsvReader(TextReader reader, IReaderConfiguration config);
}
