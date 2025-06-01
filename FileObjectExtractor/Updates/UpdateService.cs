using FileObjectExtractor.Constants;
using FileObjectExtractor.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileObjectExtractor.Updates
{
    public class UpdateService : IUpdateService
    {
        public async Task<Update> CheckForUpdate()
        {
            // GitHub "latest release" API endpoint for the repository
            string url = "https://api.github.com/repos/petercallioni/FileObjectExtractor/releases/latest";

            using (HttpClient client = new HttpClient())
            {
                // GitHub API requires a User-Agent header.
                client.DefaultRequestHeaders.Add("User-Agent", "FileObjectExtractorApp");

                // Execute the GET request synchronously.
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // Optionally log error information or throw an exception.
                    throw new Exception("Failed to retrieve update information from GitHub.");
                }

                // Read and parse the content returned from the GitHub API.
                string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                GitHubRelease? gitHubRelease = JsonSerializer.Deserialize<GitHubRelease>(json);

                if (gitHubRelease == null)
                {
                    throw new Exception("Failed to parse GitHub release information.");
                }

                // Extract the tag name (e.g., "v1.2.3") and the release URL.
                string tagName = gitHubRelease.TagName;
                string checkumsUrl = gitHubRelease.Assets.Where(x => x.Name.Equals(UpdateAssetFiles.CHECKSUMS)).FirstOrDefault()?.BrowserDownloadUrl ?? string.Empty;

                UpdateOS updateOS = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? UpdateOS.WINDOWS : UpdateOS.LINUX;
                string targetAsset = updateOS == UpdateOS.WINDOWS ? UpdateAssetFiles.WINDOWS_ARCHIVE_NAME : UpdateAssetFiles.LINUX_ARCHIVE_NAME;
                string releaseUrl = gitHubRelease.Assets.Where(x => x.Name.Equals(targetAsset)).FirstOrDefault()?.BrowserDownloadUrl ?? string.Empty;

                if (string.IsNullOrEmpty(tagName))
                {
                    throw new Exception("Invalid release information: tag_name is missing.");
                }

                return new Update(
                    releaseUrl,
                    checkumsUrl,
                    tagName,
                    updateOS,
                    gitHubRelease.Assets.Where(x => x.Name.Equals(targetAsset)).FirstOrDefault()?.Size ?? -1L,
                    gitHubRelease.CreatedAt
                    );
            }
        }

        public async Task InstallUpdate(Update update, DownloadedUpdateFiles files)
        {
            await UpdateUpdater(update, files.ArchiveFile);
            RunUpdater(update, files);
        }

        public async Task<DownloadedUpdateFiles> DownloadUpdate(Update update, IProgress<DownloadProgressReport>? progress = null)
        {
            using (HttpClient client = new HttpClient())
            {
                // Download the checksum file (assumed to be small)
                HttpResponseMessage getChecksumFile = await client.GetAsync(update.ChecksumUrl);
                getChecksumFile.EnsureSuccessStatusCode();
                byte[] checksumFileBytes = await getChecksumFile.Content.ReadAsByteArrayAsync();

                // Download the main archive, setting ResponseHeadersRead to start streaming immediately.
                HttpResponseMessage getReleaseArchive = await client.GetAsync(
                    update.ReleaseUrl, HttpCompletionOption.ResponseHeadersRead);
                getReleaseArchive.EnsureSuccessStatusCode();

                // Use the known file size from the update entity.
                long totalBytesExpected = update.Size;

                byte[] archiveFileBytes;
                using (Stream contentStream = await getReleaseArchive.Content.ReadAsStreamAsync())
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    int bufferSize = 8192;
                    byte[] buffer = new byte[bufferSize];
                    long totalBytesRead = 0;
                    int bytesRead;
                    // Read in chunks
                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await memoryStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;
                        // Update progress reporting if delegate exists.
                        if (progress != null && totalBytesExpected > 0)
                        {
                            progress.Report(new DownloadProgressReport
                            {
                                Fraction = (double)totalBytesRead / totalBytesExpected,
                                BytesDownloaded = totalBytesRead,
                                TotalBytes = totalBytesExpected
                            });
                        }
                    }
                    archiveFileBytes = memoryStream.ToArray();
                }

                // Determine the file name based on the update's OS.
                string archiveFileName = update.Os == UpdateOS.WINDOWS
                    ? UpdateAssetFiles.WINDOWS_ARCHIVE_NAME
                    : UpdateAssetFiles.LINUX_ARCHIVE_NAME;

                FileInfo checksumFile = new FileInfo(Path.Combine(
                    TemporaryFiles.GetTemporaryUpdateDirectory(update.Version).FullName,
                    UpdateAssetFiles.CHECKSUMS));
                FileInfo archiveFile = new FileInfo(Path.Combine(
                    TemporaryFiles.GetTemporaryUpdateDirectory(update.Version).FullName,
                    archiveFileName));

                // Write the downloaded files to disk.
                await File.WriteAllBytesAsync(checksumFile.FullName, checksumFileBytes);
                await File.WriteAllBytesAsync(archiveFile.FullName, archiveFileBytes);

                return new DownloadedUpdateFiles(checksumFile, archiveFile);
            }
        }

        public void RunUpdater(Update update, DownloadedUpdateFiles downloadedUpdateFiles)
        {
            string updaterName = update.Os == UpdateOS.WINDOWS ? UpdateAssetFiles.UPDATE_HELPER_WINDOWS : UpdateAssetFiles.UPDATE_HELPER_LINUX;

            if (VerifyChecksum(
                FileExtensions.ReadAllBytesReadOnly(downloadedUpdateFiles.ChecksumFile.FullName),
                FileExtensions.ReadAllBytesReadOnly(downloadedUpdateFiles.ArchiveFile.FullName),
                downloadedUpdateFiles.ArchiveFile.Name,
                out string verifiedChecksum
                ))
            {
                // Start the update helper and pass the temporary folder path
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = updaterName,
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                startInfo.ArgumentList.Add(update.UpdateDirectory.FullName);
                startInfo.ArgumentList.Add(verifiedChecksum);

                Process updaterProcess = new Process { StartInfo = startInfo };
                updaterProcess.Start();

                Environment.Exit(0); // Exit the current process
            }
            else
            {
                throw new Exception("Checksum verification failed.");
            }
        }

        public async Task<bool> UpdateUpdater(Update update, FileInfo archiveFile)
        {
            string path = Directory.GetCurrentDirectory();
            string updaterName = update.Os == UpdateOS.WINDOWS ? UpdateAssetFiles.UPDATE_HELPER_WINDOWS : UpdateAssetFiles.UPDATE_HELPER_LINUX;
            string updaterPath = Path.Combine(path, updaterName);

            FileInfo currentUpdaterApplication = new FileInfo(updaterPath);

            using (ZipArchive zipArchive = ZipFile.Open(archiveFile.FullName, ZipArchiveMode.Read))
            {
                ZipArchiveEntry? entry = zipArchive.Entries.FirstOrDefault(e => e.Name.Equals(updaterName, StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                {
                    throw new FileNotFoundException($"The file {updaterName} was not found in the archive.");
                }

                if (currentUpdaterApplication.Exists)
                {
                    currentUpdaterApplication.MoveTo(Path.Combine(path, $"{updaterName}{StringConstants.TEMP_FILE_EXTENSION}"), true);
                }

                await Task.Run(() =>
                {
                    entry.ExtractToFile(updaterPath, true);
                });
            }

            return true;
        }

        public bool VerifyChecksum(byte[] checksumFile, byte[] archiveFile, string archiveFileName, out string verifiedChecksum)
        {
            verifiedChecksum = string.Empty;

            // Convert checksum file content to a string
            string checksumFileContent = Encoding.UTF8.GetString(checksumFile);
            checksumFileContent = checksumFileContent.TrimStart('\uFEFF');

            // Split into lines
            string[] lines = Regex.Split(checksumFileContent, @"\r?\n");

            // Find the checksum for the specified archive file
            string? expectedChecksum = lines
                .Select(line => Regex.Split(line, @"\s+")) // Split by whitespace
                .Where(parts => parts.Length > 1 && parts[1].Equals(archiveFileName, StringComparison.OrdinalIgnoreCase)) // Check if the second part matches the archive file name
                .Select(parts => parts[0]) // Get the checksum (first part)
                .FirstOrDefault();

            // If the archive file isn't found in the checksum file, return false
            if (string.IsNullOrEmpty(expectedChecksum))
            {
                return false;
            }

            // Compute SHA-256 hash of the archive file
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(archiveFile);
                string computedChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                if (computedChecksum == expectedChecksum.ToLower())
                {
                    verifiedChecksum = computedChecksum;
                    return true;
                }

                return false;
            }
        }
    }
}