namespace PolicyReporter.DataHandling.Parsers;

/// <summary>
/// Represents errors that occur when parsing policies.
/// </summary>
internal class PolicyParseException : Exception
{
    /// <summary>
    /// Creates a <see cref="PolicyParseException"/> instance.
    /// </summary>
    public PolicyParseException()
        : base()
    {
    }

    /// <summary>
    /// Creates a <see cref="PolicyParseException"/> instance.
    /// </summary>
    /// <param name="message">Message describing error.</param>
    /// <param name="innerException">Inner exception.</param>
    public PolicyParseException(string message, Exception? innerException) 
        : base(message, innerException)
    {
    }
}
