using Avalonia;
using FileObjectExtractor.CLI;
using FileObjectExtractor.Models;
using FileObjectExtractor.Updates;
using System;
using System.Runtime.InteropServices;

namespace FileObjectExtractor
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0].Equals("UPDATED"))
                {
                    // This is a special case where the application is relaunched after an update.
                    Global.StartedFromUpdate = true;
                    args = Array.Empty<string>();
                }
            }

            if (args.Length > 0)
            {
                AttachConsoleOnWindows();

                FileController fileController = new FileController(null);
                UpdateService updateService = new UpdateService();
                CliController cliController = new CliController(args, fileController, updateService);
                int exitCode = (int)cliController.StartCLI();
                DetachConsoleOnWindows();

                Environment.Exit(exitCode);
            }
            else
            {
                BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
            }

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();

        // Constant for attaching to the parent process's console
        private const int ATTACH_PARENT_PROCESS = -1;

        // Import the Windows API function AttachConsole
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        // Import the Windows API function AttachConsole
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        private static void AttachConsoleOnWindows()
        {
            // Check if we are running on Windows.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Attempt to attach to the parent console.
                // This will succeed if there's a console, such as when the user runs the app from the command line.
                if (!AttachConsole(ATTACH_PARENT_PROCESS))
                {
                    // Optionally, handle the error here.
                    int errorCode = Marshal.GetLastWin32Error();
                    Console.Error.WriteLine($"Could not attach to parent console. Error code: {errorCode}");
                }
            }
        }

        private static void DetachConsoleOnWindows()
        {
            // Check if we are running on Windows.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FreeConsole();
            }
        }

        static void OnProcessExit(object? sender, EventArgs e)
        {
            Utilities.TemporaryFiles.ClearTemporaryFiles();
        }
    }
}
