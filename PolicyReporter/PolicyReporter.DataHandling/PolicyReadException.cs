namespace PolicyReporter.DataHandling.Parsers;

/// <summary>
/// Represents errors that occur when parsing policies.
/// </summary>
internal class PolicyReadException : Exception
{
    /// <summary>
    /// Creates a <see cref="PolicyReadException"/> instance.
    /// </summary>
    /// <param name="message">Message describing error.</param>
    /// <param name="innerException">Inner exception.</param>
    public PolicyReadException()
        : base()
    {
    }

    /// <summary>
    /// Creates a <see cref="PolicyReadException"/> instance.
    /// </summary>
    /// <param name="message">Message describing error.</param>
    /// <param name="innerException">Inner exception.</param>
    public PolicyReadException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
