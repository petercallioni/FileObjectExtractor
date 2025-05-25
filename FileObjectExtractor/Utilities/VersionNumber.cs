using FileObjectExtractor.Updates;
using System.Reflection;

namespace FileObjectExtractor.Utilities
{
    public static class VersionNumber
    {
        public static string VersionString()
        {
            System.Version? version = Assembly.GetExecutingAssembly().GetName().Version;

            if (version == null)
            {
                return "Unknown";
            }

            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public static Version Version()
        {
            return new Version(VersionString());
        }
    }
}
