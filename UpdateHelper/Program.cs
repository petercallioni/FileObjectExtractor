using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class Program
{
    private const string WINDOWS_ARCHIVE_NAME = "FileObjectExtractor-win-x64.zip";
    private const string LINUX_ARCHIVE_NAME = "FileObjectExtractor-linux-x64.zip";
    private const string CHECKSUMS = "checksums.txt";
    private const string MAIN_APP_WINDOWS = "FileObjectExtractor.exe";
    private const string MAIN_APP_LINUX = "FileObjectExtractor";
    private const string UPDATE_HELPER_WINDOWS = "UpdateHelper.exe";
    private const string UPDATE_HELPER_LINUX = "UpdateHelper";
    public const string TEMP_FILE_EXTENSION = ".fox_old";

    public static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: UpdateHelper <update folder> <update checksum> <NO_RESTART|RESTART>");
            return;
        }

        string tempFolder = args[0];
        string checkSumFromMainApplication = args[1];
        string restartArg = args[2];

        if (!(restartArg.Equals("NO_RESTART", StringComparison.OrdinalIgnoreCase) || restartArg.Equals("RESTART", StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Invalid argument for restart option. Use 'NO_RESTART' or 'RESTART'.");
            return;
        }

        bool noRestart = restartArg.Equals("NO_RESTART", StringComparison.OrdinalIgnoreCase);

        string mainApp = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? MAIN_APP_WINDOWS : MAIN_APP_LINUX;
        string updaterApp = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? UPDATE_HELPER_WINDOWS : UPDATE_HELPER_LINUX;
        string archive = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? WINDOWS_ARCHIVE_NAME : MAIN_APP_LINUX;

        // Find and kill the main application process
        Process[] processes = Process.GetProcessesByName(mainApp);
        foreach (Process process in processes)
        {
            try
            {
                process.Kill();
                process.WaitForExit();
                Console.WriteLine("Main application closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to close main application: {ex.Message}");
                throw;
            }
        }

        if (VerifyChecksum(
            ReadAllBytesReadOnly(GetChecksumFile(tempFolder)),
            ReadAllBytesReadOnly(GetArchiveFile(tempFolder)),
            archive,
            checkSumFromMainApplication))
        {
            ZipArchive zipArchive = ZipFile.Open(GetArchiveFile(tempFolder).FullName, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
            {
                if (!zipArchiveEntry.Name.Equals(updaterApp))
                {
                    Console.WriteLine($"Updating file: {zipArchiveEntry.Name}");
                    UpdateFile(zipArchiveEntry, Directory.GetCurrentDirectory());
                }
            }
        }
        else
        {
            Console.WriteLine("Checksum verification failed.");
            return;
        }

        if (noRestart)
        {
            Console.WriteLine("Update completed without restart.");
            return;
        }
        else
        {
            RelaunchMainApplication();
        }
    }

    public static void RelaunchMainApplication()
    {
        Console.WriteLine("Relaunching main application...");
        string mainApp = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? MAIN_APP_WINDOWS : MAIN_APP_LINUX;
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = mainApp,
            Arguments = "UPDATED",
            UseShellExecute = true
        };
        Process.Start(startInfo);

        Environment.Exit(0);
    }

    public static void UpdateFile(ZipArchiveEntry zipArchiveEntry, string directory)
    {
        string baseFileName = zipArchiveEntry.Name;
        FileInfo currentFile = new FileInfo(Path.Combine(directory, baseFileName));

        UnixFileMode? oldUnixFileMode = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? File.GetUnixFileMode(currentFile.FullName) : null;

        if (File.Exists(currentFile.FullName))
        {
            currentFile.MoveTo(Path.Combine(directory, $"{baseFileName}{TEMP_FILE_EXTENSION}"));
        }

        zipArchiveEntry.ExtractToFile(baseFileName, true);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            ChmodPlusExecute(currentFile.FullName);
        }
    }

    public static void ChmodPlusExecute(string path)
    {
        string chmodCommand = $"chmod +x \"{path}\"";

        Process process = new Process();
        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.Arguments = $"-c \"{chmodCommand}\"";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        process.WaitForExit();
    }

    public static byte[] ReadAllBytesReadOnly(FileInfo path)
    {
        byte[] fileBytes;
        using (FileStream stream = new FileStream(
            path.FullName,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite))
        {
            fileBytes = new byte[stream.Length];
            stream.ReadExactly(fileBytes);
        }
    ;

        return fileBytes;
    }

    public static FileInfo GetArchiveFile(string path)
    {
        string archiveFilePath = Path.Combine(path, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? WINDOWS_ARCHIVE_NAME : LINUX_ARCHIVE_NAME);
        if (!File.Exists(archiveFilePath))
        {
            throw new FileNotFoundException($"Archive file not found: {archiveFilePath}");
        }
        return new FileInfo(archiveFilePath);
    }

    public static FileInfo GetChecksumFile(string path)
    {
        string checksumFilePath = Path.Combine(path, CHECKSUMS);
        if (!File.Exists(checksumFilePath))
        {
            throw new FileNotFoundException($"Checksum file not found: {checksumFilePath}");
        }
        return new FileInfo(checksumFilePath);
    }

    public static bool VerifyChecksum(byte[] checksumFile, byte[] archiveFile, string archiveFileName, string checksumFromMainApplication)
    {
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

            // Compare computed checksum with expected checksum
            return computedChecksum == expectedChecksum.ToLower() && computedChecksum == checksumFromMainApplication.ToLower();
        }
    }
}