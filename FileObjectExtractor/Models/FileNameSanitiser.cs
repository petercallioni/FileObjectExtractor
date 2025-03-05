using System;
using System.IO;
using System.Text;

namespace FileObjectExtractor.Models
{
    public class FilenameSanitiser
    {
        public bool SanitiseFilename(StringBuilder sb)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            string invalidChars = GetInvalidFileNameChars();
            bool substitutionsMade = false;

            for (int i = 0; i < sb.Length; i++)
            {
                if (invalidChars.Contains(sb[i]))
                {
                    sb[i] = '_';
                    substitutionsMade = true;
                }
            }

            return substitutionsMade;
        }

        private static string GetInvalidFileNameChars()
        {
            // Get the invalid file name characters for the current operating system
            char[] invalidChars = Path.GetInvalidFileNameChars();

            // Convert to a string for easier checking
            return new string(invalidChars);
        }

    }
}