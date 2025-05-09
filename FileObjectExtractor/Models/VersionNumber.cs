using System.Reflection;

namespace FileObjectExtractor.Models
{
    public static class VersionNumber
    {
        public static string GetVersion()
        {
            System.Version? version = Assembly.GetExecutingAssembly().GetName().Version;

            if (version == null)
            {
                return "Unknown";
            }

            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
