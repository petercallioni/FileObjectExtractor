using MicrosoftObjectExtractor.Models.EMF;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace MicrosoftObjectExtractor.Models
{
    public class FileReader
    {
        /// <summary>
        /// Opens up the file and gets the initial list of objects.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public List<ExtractedFile> ParseFile(string filePath)
        {
            List<ZipArchiveEntry> embeddedFiles = new List<ZipArchiveEntry>();
            List<ExtractedFile> files = new List<ExtractedFile>();
            Dictionary<string, string> rIdsAndFiles = new Dictionary<string, string>();
            Dictionary<string, string> rIdsIconsAndFiles = new Dictionary<string, string>();

            using (FileStream file = File.OpenRead(filePath))
            using (ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.FullName.EndsWith("document.xml"))
                    {
                        rIdsIconsAndFiles = ParseDocumentFile(entry);
                    }
                    else if (entry.FullName.EndsWith("document.xml.rels"))
                    {
                        rIdsAndFiles = ParseRelsFile(entry);
                    }
                    else if (entry.FullName.Contains("embeddings") || entry.FullName.Contains("media"))
                    {
                        embeddedFiles.Add(entry);
                    }
                }

                files = CombineLists(rIdsIconsAndFiles, rIdsAndFiles, embeddedFiles);

                EmfParser emfParser = new EmfParser();
                foreach (ExtractedFile extractFile in files)
                {
                    EmfFile emfFile = emfParser.Parse(extractFile.IconFile);
                    extractFile.FileName = emfFile.GetTextContent();
                }
            }

            return files;
        }

        private List<ExtractedFile> CombineLists(Dictionary<string, string> iconRids, Dictionary<string, string> fileRids, List<ZipArchiveEntry> archiveFiles)
        {
            List<ExtractedFile> extractedFiles = iconRids.Select(key =>
            {
                string icon_path = fileRids[key.Key];
                string file_path = fileRids[key.Value];

                return new ExtractedFile(
                    archiveFiles.First(x => x.FullName.EndsWith(icon_path)),
                    archiveFiles.First(x => x.FullName.EndsWith(file_path))
                );
            }).ToList();

            EmfParser parser = new EmfParser();

            foreach (ExtractedFile file in extractedFiles)
            {
                EmfFile emfFile = parser.Parse(file.IconFile);
                StringBuilder returnText = new StringBuilder();
                foreach (EmfTextRecord textRecord in emfFile.EmfTextRecords)
                {
                    returnText.Append(textRecord.OutputString.Value);
                }
                file.FileName = returnText.ToString();
            }

            return extractedFiles;
        }

        private Dictionary<string, string> ParseRelsFile(ZipArchiveEntry archiveEntry)
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

        private Dictionary<string, string> ParseDocumentFile(ZipArchiveEntry archiveEntry)
        {
            Dictionary<string, string> rIds = new Dictionary<string, string>();

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

                        if ((fileRIdAttribute != null && fileRIdAttribute.Value.Length > 0) && (iconRIdAttribute != null && iconRIdAttribute.Value.Length > 0))
                        {
                            rIds.Add(iconRIdAttribute.Value, fileRIdAttribute.Value);
                        }
                    }
                }
            }

            return rIds;
        }
    }
}