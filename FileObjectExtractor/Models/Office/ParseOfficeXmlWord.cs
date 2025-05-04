using FileObjectExtractor.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace FileObjectExtractor.Models.Office
{
    public class ParseOfficeXmlWord : ParseOfficeXml
    {
        public ParseOfficeXmlWord()
        {
            OfficeType = OfficeType.WORD;
        }

        /// <summary>
        /// Opens up the file and gets the initial list of objects.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public override List<ExtractedFile> GetExtractedFiles(Uri filePath)
        {
            List<ZipArchiveEntry> embeddedFiles = new List<ZipArchiveEntry>();
            List<ExtractedFile> files = new List<ExtractedFile>();
            Dictionary<string, string> rIdsAndFiles = new Dictionary<string, string>();
            Dictionary<string, OleObject> rIdsIconsAndFiles = new Dictionary<string, OleObject>();

            byte[] inputFile = File.ReadAllBytes(filePath.UnescapedString());
            ThrowIfPassworded(inputFile);

            using (MemoryStream byteStream = new MemoryStream(inputFile))
            using (ZipArchive zip = new ZipArchive(byteStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.FullName.EndsWith("word/document.xml"))
                    {
                        rIdsIconsAndFiles = ParseDocumentFile(entry);
                    }
                    else if (entry.FullName.EndsWith("word/_rels/document.xml.rels"))
                    {
                        rIdsAndFiles = ParseRelsFile(entry);
                    }
                    else if (entry.FullName.Contains("embeddings") || entry.FullName.Contains("media"))
                    {
                        embeddedFiles.Add(entry);
                    }
                }

                files = CombineLists(rIdsIconsAndFiles, rIdsAndFiles, embeddedFiles);
            }

            return files;
        }

        private Dictionary<string, OleObject> ParseDocumentFile(ZipArchiveEntry archiveEntry)
        {
            Dictionary<string, OleObject> rIds = new Dictionary<string, OleObject>();

            using (Stream stream = archiveEntry.Open())
            {
                XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
                xmlDoc.Load(stream); // Load the XML document from the specified file

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
                nsmgr.AddNamespace("v", "urn:schemas-microsoft-com:vml");
                nsmgr.AddNamespace("o", "urn:schemas-microsoft-com:office:office");

                XmlNodeList wObjects = xmlDoc.GetElementsByTagName("w:object");

                foreach (XmlNode wObject in wObjects)
                {
                    XmlNode? vImageData = wObject.SelectSingleNode(".//v:imagedata", nsmgr);
                    XmlNode? oOleObject = wObject.SelectSingleNode(".//o:OLEObject[@Type='Embed']", nsmgr);

                    if (oOleObject?.Attributes != null && vImageData?.Attributes != null)
                    {
                        XmlAttribute? fileRIdAttribute = oOleObject.Attributes["r:id"];
                        XmlAttribute? iconRIdAttribute = vImageData.Attributes["r:id"];
                        XmlAttribute? drawAspect = oOleObject.Attributes["DrawAspect"];

                        bool hasIcon = drawAspect?.Value == "Icon";

                        if (fileRIdAttribute != null && fileRIdAttribute.Value.Length > 0 && iconRIdAttribute != null && iconRIdAttribute.Value.Length > 0)
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