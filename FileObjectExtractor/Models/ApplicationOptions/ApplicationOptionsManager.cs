namespace FileObjectExtractor.Models.ApplicationOptions
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text.Json;

    public static class ApplicationOptionsManager
    {
        public static FileInfo GetApplicationOptionsFile()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string? appDataFolderFoxParentDir = Assembly.GetExecutingAssembly().GetName().Name;

            if (appDataFolderFoxParentDir != null)
            {
                appDataFolder = Path.Combine(appDataFolder, appDataFolderFoxParentDir);
            }

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            string optionsFilePath = Path.Combine(appDataFolder, "ApplicationOptions.json");
            return new FileInfo(optionsFilePath);
        }

        public static void SaveOptions(ApplicationOptions options)
        {
            FileInfo optionsFile = GetApplicationOptionsFile();
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true // nice formatting
            };
            string json = JsonSerializer.Serialize(options, jsonOptions);
            File.WriteAllText(optionsFile.FullName, json);
        }

        public static ApplicationOptions LoadOptions()
        {
            FileInfo optionsFile = GetApplicationOptionsFile();
            if (!optionsFile.Exists)
            {
                // Return default options if the file hasn't been created yet.
                return new ApplicationOptions();
            }
            string json = File.ReadAllText(optionsFile.FullName);
            ApplicationOptions? options = JsonSerializer.Deserialize<ApplicationOptions>(json);
            // Ensure non-null to avoid issues later on.
            return options ?? new ApplicationOptions();
        }
    }
}
