namespace FileObjectExtractor.Models.ApplicationOptions
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text.Json;

    public static class ApplicationOptionsManager
    {
        // Lazy initialization ensures the options are loaded only once (thread-safe).
        private static readonly Lazy<ApplicationOptions> options =
             new Lazy<ApplicationOptions>(() => LoadOptions(), isThreadSafe: true);

        /// <summary>
        /// Gets the application options instance.
        /// </summary>
        public static ApplicationOptions Options => options.Value;

        /// <summary>
        /// Returns the file path for the options file.
        /// </summary>
        public static FileInfo GetApplicationOptionsFile()
        {
            // Get the user's Application Data folder.
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Use the executing assembly's name as a sub-folder
            string? appFolderName = Assembly.GetExecutingAssembly().GetName().Name;
            if (appFolderName != null)
            {
                appDataFolder = Path.Combine(appDataFolder, appFolderName);
            }

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            string optionsFilePath = Path.Combine(appDataFolder, "ApplicationOptions.json");
            return new FileInfo(optionsFilePath);
        }

        /// <summary>
        /// Saves the given ApplicationOptions to disk in JSON format.
        /// </summary>
        public static void SaveOptions(ApplicationOptions options)
        {
            FileInfo optionsFile = GetApplicationOptionsFile();

            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true // for nicer formatting
            };

            string json = JsonSerializer.Serialize(options, jsonOptions);
            File.WriteAllText(optionsFile.FullName, json);
        }

        /// <summary>
        /// Loads the ApplicationOptions from disk or returns a new instance if none exist.
        /// </summary>
        private static ApplicationOptions LoadOptions()
        {
            FileInfo optionsFile = GetApplicationOptionsFile();

            if (!optionsFile.Exists)
            {
                return new ApplicationOptions();
            }

            string json = File.ReadAllText(optionsFile.FullName);
            ApplicationOptions? options = JsonSerializer.Deserialize<ApplicationOptions>(json);

            // Return a new instance if the deserialization failed.
            return options ?? new ApplicationOptions();
        }
    }
}
