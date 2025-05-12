using FileObjectExtractor.Constants;
using FileObjectExtractor.Extensions;
using FileObjectExtractor.Models.EMF;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace FileObjectExtractor.Models.Office
{
    public abstract class ParseOfficeXml : ParseOffice
    {
        private const string EMPTY_NAME = "UNKNOWN";
        protected void ThrowIfPassworded(byte[] bytes)
        {
            if (bytes.Take(8).SequenceEqual(new byte[] { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 })) // OLECF file signature, instead of expected PK header
            {
                throw new NotSupportedException("The file is password protected; please remove the password and try again.");
            }
        }

        protected List<ExtractedFile> CombineLists(
    Dictionary<string, OleObject> iconRids,
    Dictionary<string, string> fileRids,
    List<ZipArchiveEntry> embeddedFiles,
    List<ZipArchiveEntry> mediaFiles)
        {
            int documentOrderCounter = 0;
            List<ExtractedFile> files = new List<ExtractedFile>();
            EmfParser parser = new EmfParser();

            foreach (string key in iconRids.Keys)
            {
                // Retrieve the related values from the dictionaries
                OleObject currentOle = iconRids[key];
                string iconPath = fileRids[key];
                string filePath = fileRids[currentOle.Rid];
                bool hasIcon = currentOle.HasIcon;

                // Get the corresponding archive entries for the icon & file
                ZipArchiveEntry iconEntry = GetEntry(iconPath, mediaFiles);
                ZipArchiveEntry fileEntry = GetEntry(filePath, embeddedFiles);

                // Determine the explicit display name by checking if it is an EMF file
                string explicitName = IsEmf(iconEntry.Name)
                    ? parser.Parse(iconEntry.GetBytes()).GetTextContent()
                    : iconEntry.Name;

                bool hasExplicitName = !string.IsNullOrWhiteSpace(explicitName);
                string fileDisplayName = hasExplicitName ? explicitName : EMPTY_NAME;

                // Create the new ExtractedFile instance based on the file entry.
                ExtractedFile extractedFile = new ExtractedFile(fileEntry);

                // If no explicit name was provided, try guessing the extension using magic bytes.
                if (fileDisplayName.Equals(EMPTY_NAME))
                {
                    if (MagicBytes.FileType.GuessFileType(fileEntry.GetBytes(), out string extension))
                    {
                        fileDisplayName += extension;
                        extractedFile.FileNameWarnings.Add(StringConstants.WARNINGS.GUESSED_EXTENSION);
                    }
                }

                extractedFile.FileName = fileDisplayName;
                extractedFile.DocumentOrder = documentOrderCounter++;

                // If the file either has no icon or lacked an explicit name, add a warning.
                if (!hasIcon || !hasExplicitName)
                {
                    extractedFile.FileNameWarnings.Add(StringConstants.WARNINGS.NO_EXPLICIT_NAME);
                }

                files.Add(extractedFile);
            }

            return files;
        }

        private ZipArchiveEntry GetEntry(string path, List<ZipArchiveEntry> embeddedFiles)
        {
            // Strip the first element and try to find a match within the archive entries
            string checkPath = StripFirstElement(path);
            ZipArchiveEntry? entry = embeddedFiles.FirstOrDefault(x => x.FullName.EndsWith(checkPath));
            if (entry == null)
            {
                throw new InvalidOperationException($"Could not find archive entry ending with '{checkPath}'.");
            }
            return entry;
        }

        private bool IsEmf(string file)
        {
            if (file.EndsWith(".emf", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".wmf", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        protected Dictionary<string, string> ParseRelsFile(ZipArchiveEntry archiveEntry)
        {
            Dictionary<string, string> relIds = new Dictionary<string, string>();

            using (Stream stream = archiveEntry.Open())
            {
                XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
                xmlDoc.Load(stream); // Load the XML document from the specified file

                XmlNodeList relationships = xmlDoc.GetElementsByTagName("Relationship");

                foreach (XmlNode relationship in relationships)
                {
                    string id = "";
                    string target = "";

                    if (relationship.Attributes != null)
                    {
                        foreach (XmlAttribute attribute in relationship.Attributes)
                        {
                            if (attribute.Name.Equals("Id"))
                            {
                                id = attribute.Value;
                            }
                            else if (attribute.Name.Equals("Target"))
                            {
                                target = attribute.Value;
                            }
                        }
                    }

                    if (id.Length > 0 && target.Length > 0)
                    {
                        relIds.Add(id, target);
                    }
                }
            }

            return relIds;
        }

        private string StripFirstElement(string input)
        {
            return input.Substring(input.IndexOf('/'));
        }
    }
}