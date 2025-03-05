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
        protected void ThrowIfPassworded(byte[] bytes)
        {
            if (bytes.Take(8).SequenceEqual(new byte[] { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 })) // OLECF file signature, instead of expected PK header
            {
                throw new NotSupportedException("The file is password protected; please remove the password and try again.");
            }
        }

        protected List<ExtractedFile> CombineLists(Dictionary<string, OleObject> iconRids, Dictionary<string, string> fileRids, List<ZipArchiveEntry> archiveFiles)
        {
            List<ExtractedFile> files = new List<ExtractedFile>();

            EmfParser parser = new EmfParser();

            foreach (string key in iconRids.Keys)
            {
                string iconPath = fileRids[key];
                string filePath = fileRids[iconRids[key].Rid];
                bool hasIcon = iconRids[key].HasIcon;

                ZipArchiveEntry iconEntry = archiveFiles.First(x => x.FullName.EndsWith(StripFirstElement(iconPath)));
                ZipArchiveEntry fileEntry = archiveFiles.First(x => x.FullName.EndsWith(StripFirstElement(filePath)));

                EmfFile emfFile = parser.Parse(iconEntry.GetBytes());
                ExtractedFile extractedFile = new ExtractedFile(fileEntry)
                {
                    FileName = emfFile.GetTextContent(),
                };

                if (!hasIcon)
                {
                    extractedFile.FileNameWarnings.Add(StringConstants.WARNINGS.NO_EXPLICIT_NAME);
                }

                files.Add(extractedFile);
            }

            return files;
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