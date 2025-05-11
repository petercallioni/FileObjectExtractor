using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileObjectExtractor.Models.MagicBytes
{
    public class FileType
    {
        private static Dictionary<string, FileEntry> fileEntriesCache = null!; // Cache for file entries to avoid reloading from JSON every time.

        /// <summary>
        /// Guesses the file type based on the magic bytes in the provided byte array. Extension is returned as an output parameter, including preceeding "."..
        /// </summary>
        /// <param name="data"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static bool GuessFileType(byte[] data, out string extension)
        {
            extension = string.Empty;
            foreach (KeyValuePair<string, FileEntry> entry in GetFileEntries())
            {
                string key = entry.Key;
                FileEntry fileEntry = entry.Value;
                foreach (Sign sign in fileEntry.Signs)
                {
                    // Check if the data starts with the hex bytes defined in the sign
                    if (data.Length >= sign.Offset + sign.HexBytes.Length &&
                        ByteArrayCompare(data, sign.Offset, sign.HexBytes))
                    {
                        extension = $".{key}";
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Reads the JSON file containing the magic bytes and their corresponding file types.
        /// Uses singleton pattern to cache the file entries for performance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="JsonException"></exception>
        private static Dictionary<string, FileEntry> GetFileEntries()
        {
            if (fileEntriesCache != null)
            {
                return fileEntriesCache;
            }

            // Thanks to https://gist.github.com/Qti3e/6341245314bf3513abb080677cd1c93b#file-readme-md
            string resourceName = "FileObjectExtractor.Assets.extensions.json"; // adjust this as necessary
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Resource '{resourceName}' not found in assembly.");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string jsonContent = reader.ReadToEnd();

                    // Assuming you have a class that maps to your JSON structure.
                    Dictionary<string, FileEntry>? fileEntries = JsonSerializer.Deserialize<Dictionary<string, FileEntry>>(jsonContent);

                    if (fileEntries != null)
                    {
                        fileEntriesCache = fileEntries;
                        return fileEntriesCache;
                    }
                    else
                    {
                        throw new JsonException("Failed to deserialize JSON content.");
                    }
                }
            }
        }

        /// <summary>
        /// Compares a byte array with a hex byte array at a specific offset.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="hexBytes"></param>
        /// <returns></returns>
        private static bool ByteArrayCompare(byte[] data, int offset, byte[] hexBytes)
        {
            for (int i = 0; i < hexBytes.Length; i++)
            {
                if (data[offset + i] != hexBytes[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    public record FileEntry(
        [property: JsonPropertyName("signs")] List<Sign> Signs,
        [property: JsonPropertyName("mime")] string Mime);
}
