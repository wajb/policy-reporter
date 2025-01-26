
namespace PolicyReporter.DataHandling;

/// <summary>
/// Describes a file that can be used as a source of policy data for ingestion.
/// </summary>
/// <param name="filePath">Path of file.</param>
/// <param name="sourceId">Identifier of source.</param>
public class FilePolicySource(string filePath, int sourceId) : IPolicySource
{
    /// <inheritdoc/>
    public int SourceId => sourceId;

    /// <inheritdoc/>
    StreamReader IPolicySource.GetReader()
    {
        try
        {
            return new StreamReader(Path.Join(PathUtils.GetRootPath(), filePath));
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException or IOException)
        {
            throw new PolicyReadException($"Failed to read \"{filePath}\" for source ID {SourceId}.", e);
        }
    }
}
