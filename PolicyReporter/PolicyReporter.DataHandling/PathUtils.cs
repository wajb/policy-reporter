using System.Reflection;

namespace PolicyReporter.DataHandling;

/// <summary>
/// File path utility.
/// </summary>
public static class PathUtils
{
    const string RootFolderName = "PolicyReporter";

    /// <summary>
    /// Resolves a common directory path that all projects can access.
    /// </summary>
    /// <remarks>This wouldn't be necessary in production since file paths could be provided as absolute values.</remarks>
    /// <returns>Path to common directory.</returns>
    public static string GetRootPath()
    {
        DirectoryInfo directory = new(Assembly.GetEntryAssembly()!.Location);

        while (directory.Name != RootFolderName && directory.Parent is not null)
        {
            directory = directory.Parent;
        }
        
        return directory.FullName;
    }
}
