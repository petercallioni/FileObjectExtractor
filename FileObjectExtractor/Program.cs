using Avalonia;
using FileObjectExtractor.CLI;
using FileObjectExtractor.Models;
using FileObjectExtractor.Services;
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
                bool attachedToParent = false;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Try to attach to the parent's console and remember if it succeeded.
                    attachedToParent = AttachConsole(ATTACH_PARENT_PROCESS);
                    if (!attachedToParent)
                    {
                        AllocConsole();
                    }
                }

                try
                {
                    FileController fileController = new FileController(null);
                    UpdateService updateService = new UpdateService();
                    CliProgressService cliProgressService = new CliProgressService();
                    CliController cliController = new CliController(args, fileController, updateService, cliProgressService);
                    int exitCode = (int)cliController.StartCLI();

                    Environment.Exit(exitCode);
                }
                finally
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        // Only free the console if we allocated it.
                        if (!attachedToParent)
                        {
                            FreeConsole();
                        }
                    }
                }
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
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        static void OnProcessExit(object? sender, EventArgs e)
        {
            Utilities.TemporaryFiles.ClearTemporaryFiles();
        }
    }
}
