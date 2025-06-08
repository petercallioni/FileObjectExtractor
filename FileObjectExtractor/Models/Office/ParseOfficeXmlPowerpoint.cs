using FileObjectExtractor.Models.Office;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace FileObjectExtractor.Models
{
    public class ParseOfficeXmlPowerpoint : ParseOfficeXml
    {
        public ParseOfficeXmlPowerpoint()
        {
            OfficeType = OfficeType.POWERPOINT;
        }

        /// <summary>
        /// Opens up the file and gets the initial list of objects.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public override List<ExtractedFile> GetExtractedFiles(byte[] inputFile)
        {
            List<ZipArchiveEntry> embeddedFiles = new List<ZipArchiveEntry>();
            List<ZipArchiveEntry> mediaFiles = new List<ZipArchiveEntry>();
            List<ExtractedFile> files = new List<ExtractedFile>();

            ThrowIfPassworded(inputFile);

            using (MemoryStream byteStream = new MemoryStream(inputFile))
            using (ZipArchive zip = new ZipArchive(byteStream, ZipArchiveMode.Read))
            {
                List<ZipArchiveEntry> sheetEntries = new List<ZipArchiveEntry>();
                Dictionary<string, ZipArchiveEntry> relsEntries = new Dictionary<string, ZipArchiveEntry>();

                // Collect sheet and relationship entries
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.FullName.Contains("slide") && entry.FullName.EndsWith(".xml"))
                    {
                        sheetEntries.Add(entry);
                    }
                    else if (entry.FullName.Contains("slide") && entry.FullName.EndsWith(".xml.rels"))
                    {
                        relsEntries[entry.Name] = entry;
                    }
                    else if (entry.FullName.Contains("embeddings"))
                    {
                        embeddedFiles.Add(entry);
                    }
                    else if (entry.FullName.Contains("media"))
                    {
                        mediaFiles.Add(entry);
                    }
                }

                // Process each sheet and its corresponding relationship file
                foreach (ZipArchiveEntry sheetEntry in sheetEntries)
                {
                    string relsFileName = sheetEntry.Name + ".rels";
                    if (relsEntries.ContainsKey(relsFileName))
                    {
                        Dictionary<string, OleObject> rIdsIconsAndFiles = ParseSheetFile(sheetEntry);
                        Dictionary<string, string> rIdsAndFiles = ParseRelsFile(relsEntries[relsFileName]);
                        files.AddRange(CombineLists(rIdsIconsAndFiles, rIdsAndFiles, embeddedFiles, mediaFiles));
                    }
                }
            }

            return files;
        }
        /// <summary>
        /// Opens up the file and gets the initial list of objects.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public override List<ExtractedFile> GetExtractedFiles(Uri filePath)
        {
            byte[] inputFile = OpenOfficeFile(filePath);
            return GetExtractedFiles(inputFile);
        }

        private Dictionary<string, OleObject> ParseSheetFile(ZipArchiveEntry archiveEntry)
        {
            Dictionary<string, OleObject> rIds = new Dictionary<string, OleObject>();

            using (Stream stream = archiveEntry.Open())
            {
                XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
                xmlDoc.Load(stream); // Load the XML document from the specified file

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
                nsmgr.AddNamespace("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                nsmgr.AddNamespace("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

                XmlNodeList oleObjects = xmlDoc.GetElementsByTagName("p:oleObj");

                foreach (XmlNode oleObject in oleObjects)
                {
                    XmlNode? blip = oleObject.SelectSingleNode(".//a:blip", nsmgr);

                    if (blip?.Attributes != null && oleObject?.Attributes != null)
                    {
                        XmlAttribute? fileRIdAttribute = oleObject.Attributes["r:id"];
                        XmlAttribute? iconRIdAttribute = blip.Attributes["r:embed"];
                        XmlAttribute? showAsIcon = oleObject.Attributes["showAsIcon"];

                        bool hasIcon = showAsIcon?.Value == "1";

                        if ((fileRIdAttribute != null && fileRIdAttribute.Value.Length > 0) && (iconRIdAttribute != null && iconRIdAttribute.Value.Length > 0))
                        {
                            rIds.Add(iconRIdAttribute.Value, new OleObject(fileRIdAttribute.Value, hasIcon));
                        }
                    }
                }
            }

            return rIds;
        }
    }
}