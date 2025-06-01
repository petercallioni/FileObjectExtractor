using FileObjectExtractor.Utilities;
using System;
using System.IO;

namespace FileObjectExtractor.Updates
{
    public class Update
    {
        private readonly Uri releaseUrl;
        private readonly Uri checksumUrl;
        private readonly Version version;
        private readonly DirectoryInfo updateDirectory;
        private readonly UpdateOS os;
        private readonly bool isUpgrade;
        private readonly long size;
        private readonly DateTime createdAt;

        public Update(string releaseUrl, string checksumUrl, string version, UpdateOS os, long size, DateTime createdAt)
        {

            this.checksumUrl = new Uri(checksumUrl);
            this.releaseUrl = new Uri(releaseUrl);
            this.version = new Version(version);
            this.isUpgrade = VersionNumber.Version() < this.version;
            this.os = os;
            updateDirectory = TemporaryFiles.GetTemporaryUpdateDirectory(version);
            this.size = size;
            this.createdAt = createdAt;
        }

        public Uri ReleaseUrl => releaseUrl;
        public Uri ChecksumUrl => checksumUrl;
        public Version Version => version;
        public DirectoryInfo UpdateDirectory => updateDirectory;
        public UpdateOS Os => os;

        public bool IsUpgrade => isUpgrade;

        public long Size => size;

        public DateTime CreatedAt => createdAt;
    }
}
