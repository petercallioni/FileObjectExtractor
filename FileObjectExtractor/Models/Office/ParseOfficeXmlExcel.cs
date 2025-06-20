﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace FileObjectExtractor.Models.Office
{
    public class ParseOfficeXmlExcel : ParseOfficeXml
    {
        public ParseOfficeXmlExcel()
        {
            OfficeType = OfficeType.EXCEL;
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
                    if (entry.FullName.Contains("sheet") && entry.FullName.EndsWith(".xml"))
                    {
                        sheetEntries.Add(entry);
                    }
                    else if (entry.FullName.Contains("sheet") && entry.FullName.EndsWith(".xml.rels"))
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

                XmlNodeList oleObjects = xmlDoc.GetElementsByTagName("objectPr");

                foreach (XmlNode oleObject in oleObjects)
                {
                    XmlAttribute? iconRIdAttribute = oleObject?.Attributes?["r:id"];
                    XmlAttribute? dvAspectIcon = oleObject?.ParentNode?.Attributes?["dvAspect"];
                    XmlAttribute? fileRIdAttribute = oleObject?.ParentNode?.Attributes?["r:id"];

                    bool hasIcon = dvAspectIcon?.Value == "DVASPECT_ICON";

                    if (oleObject?.ParentNode?.LocalName == "oleObject")
                    {
                        if (fileRIdAttribute != null && iconRIdAttribute != null)
                        {
                            string fileRIdValue = fileRIdAttribute.Value;
                            string iconRIdValue = iconRIdAttribute.Value;

                            if (!string.IsNullOrEmpty(fileRIdValue) && !string.IsNullOrEmpty(iconRIdValue))
                            {
                                rIds.Add(iconRIdValue, new OleObject(fileRIdValue, hasIcon));
                            }
                        }
                    }
                }
            }

            return rIds;
        }
    }
}