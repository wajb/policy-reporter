using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace PolicyReporter.DataHandling.Parsers;

/// <summary>
/// Strategy that parses policies with <typeparamref name="T"/> format from CSV files.
/// </summary>
/// <typeparam name="T">Type representing source policy format.</typeparam>
/// <param name="csvReaderFactory">Factory to create <see cref="CsvReader"/>s.</param>
public abstract class CsvPolicyParser<T>(ICsvReaderFactory csvReaderFactory) : IPolicyParserStrategy where T : class
{
    /// <inheritdoc/>
    public async IAsyncEnumerable<Policy> ParsePolicies(IPolicySource source)
    {
        using StreamReader reader = source.GetReader();
        using CsvReader csv = csvReaderFactory.CreateCsvReader(reader, CsvConfiguration.FromAttributes<T>());

        IAsyncEnumerator<T> enumerator = csv.GetRecordsAsync<T>().GetAsyncEnumerator();

        while (true)
        {
            T? record = null;

            try
            {
                if (!(await enumerator.MoveNextAsync()))
                    yield break;

                record = enumerator.Current;
            }
            catch (HeaderValidationException e)
            {
                throw new PolicyParseException($"Failed to parse CSV source with {GetType().Name} strategy.", e);
            }
            catch (ReaderException e)
            {
                throw new PolicyParseException($"Failed to parse policy record value.", e);
            }

            yield return MapToPolicy(record, source.SourceId) with
            {
                Currency = GetCurrencyCode()
            };
        }
    }

    /// <summary>
    /// Converts a <typeparamref name="T"/> to a <see cref="Policy"/>.
    /// </summary>
    /// <param name="record">Source policy.</param>
    /// <param name="sourceId">Identifier of source.</param>
    /// <returns>Normalised policy.</returns>
    protected abstract Policy MapToPolicy(T record, int sourceId);

    private string GetCurrencyCode()
    {
        CultureInfo culture = typeof(T).GetCustomAttribute<CultureInfoAttribute>()!.CultureInfo;

        return new RegionInfo(culture.LCID).ISOCurrencySymbol;
    }
}
