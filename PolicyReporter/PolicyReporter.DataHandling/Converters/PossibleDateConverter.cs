

using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;

namespace PolicyReporter.DataHandling.Converters;

/// <summary>
/// Type converter for <see cref="DateOnly"/> that converts non date values to <see langword="null"/>.
/// </summary>
public class PossibleDateConverter : DefaultTypeConverter
{
    /// <inheritdoc/>
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData) =>
        DateOnly.TryParse(text, out DateOnly result) ? result : null;
}
