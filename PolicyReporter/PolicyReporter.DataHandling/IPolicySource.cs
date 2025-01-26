namespace PolicyReporter.DataHandling;

/// <summary>
/// Describes a source of policy data for ingestion.
/// </summary>
public interface IPolicySource
{
    /// <summary>
    /// Identifier of source (broker).
    /// </summary>
    int SourceId { get; }

    /// <summary>
    /// Gets a stream for reading this source. Dispose once reading is complete.
    /// </summary>
    /// <returns><see cref="StreamReader"/> instance.</returns>
    StreamReader GetReader();
}
