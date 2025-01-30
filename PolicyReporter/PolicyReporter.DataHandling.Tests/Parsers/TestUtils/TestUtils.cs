namespace PolicyReporter.DataHandling.Tests.Parsers.TestUtils;

public static class TestUtils
{
    /// <summary>
    /// Gets an <see cref="IAsyncEnumerator{T}"/> that throws on its first iteration.
    /// </summary>
    /// <param name="exception">Exception to throw.</param>
    /// <returns>Primed async enumerator.</returns>
    public static async IAsyncEnumerable<T> GetAsyncEnumeratorThatThrows<T>(Exception exception)
    {
        await Task.CompletedTask;
        throw exception;

#pragma warning disable CS0162 // Unreachable code detected
        yield break;
#pragma warning restore CS0162 // Unreachable code detected
    }
}
