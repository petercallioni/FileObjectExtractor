using FileObjectExtractor.Updates;
using System.Reflection;

namespace FileObjectExtractor.Utilities
{
    public static class VersionNumber
    {
        /// <summary>
        /// Retrieves the version string of the currently executing assembly. Does not include a leading 'v'.
        /// </summary>
        /// <returns>A string representing the version of the assembly in the format "Major.Minor.Build". Returns "Unknown" if
        /// the version information is unavailable.</returns>
        public static string VersionString()
        {
            System.Version? version = Assembly.GetExecutingAssembly().GetName().Version;

            if (version == null)
            {
                return "Unknown";
            }

            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
        /// <summary>
        /// Retrieves the current version of the application.
        /// </summary>
        /// <returns>A <see cref="Version"/> object representing the current version of the application.</returns>
        public static Version Version()
        {
            return new Version(VersionString());
        }
    }
}
