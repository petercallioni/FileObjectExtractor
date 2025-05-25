using System;

namespace FileObjectExtractor.Updates
{
    public class Version : IComparable<Version>
    {
        // Read-only properties
        public int Major { get; }
        public int Minor { get; }
        public int Revision { get; }

        // Constructor taking three integers
        public Version(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        // Constructor that accepts a string like "v1.2.3"
        public Version(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Version string cannot be null or empty.", nameof(version));

            // Remove leading 'v' if present (case-insensitive)
            if (version.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                version = version.Substring(1);

            string[] parts = version.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Version string must be in the format 'vMajor.Minor.Revision'", nameof(version));

            if (!int.TryParse(parts[0], out int major))
                throw new FormatException("Invalid major version number.");
            if (!int.TryParse(parts[1], out int minor))
                throw new FormatException("Invalid minor version number.");
            if (!int.TryParse(parts[2], out int revision))
                throw new FormatException("Invalid revision version number.");

            Major = major;
            Minor = minor;
            Revision = revision;
        }

        // Override ToString to return a formatted version string
        public override string ToString()
        {
            return $"v{Major}.{Minor}.{Revision}";
        }

        // Override Equals for value equality
        public override bool Equals(object? obj)
        {
            if (obj is Version other)
            {
                return Major == other.Major &&
                       Minor == other.Minor &&
                       Revision == other.Revision;
            }
            return false;
        }

        // Optionally, provide a type-safe Equals method
        public bool Equals(Version? other)
        {
            if (ReferenceEquals(other, null))
                return false;
            return Major == other.Major &&
                   Minor == other.Minor &&
                   Revision == other.Revision;
        }

        // Override GetHashCode based on the three properties
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Revision);
        }

        // Implement IComparable for comparing two Version instances
        public int CompareTo(Version? other)
        {
            if (other == null)
                return 1;

            // Compare Major numbers first, then Minor, then Revision
            int majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0)
                return majorComparison;

            int minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0)
                return minorComparison;

            return Revision.CompareTo(other.Revision);
        }

        // Overload equality operator
        public static bool operator ==(Version? left, Version? right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left is null)
                return false;
            return left.Equals(right);
        }

        // Overload inequality operator
        public static bool operator !=(Version? left, Version? right)
        {
            return !(left == right);
        }

        // Overload less than operator
        public static bool operator <(Version? left, Version? right)
        {
            if (left is null)
                throw new ArgumentNullException(nameof(left));
            return left.CompareTo(right) < 0;
        }

        // Overload greater than operator
        public static bool operator >(Version? left, Version? right)
        {
            if (left is null)
                throw new ArgumentNullException(nameof(left));
            return left.CompareTo(right) > 0;
        }
    }
}
