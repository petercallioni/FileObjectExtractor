using FileObjectExtractor.Extensions;
using FileObjectExtractor.Interfaces;
using FileObjectExtractor.Models.EMF;
using FileObjectExtractor.Models.EMF.EmfPart;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace FileObjectExtractor.Models
{
    public abstract class ParseOfficeXml : IParseOffice
    {
        public abstract List<ExtractedFile> GetExtractedFiles(string filePath);

        protected List<ExtractedFile> CombineLists(Dictionary<string, string> iconRids, Dictionary<string, string> fileRids, List<ZipArchiveEntry> archiveFiles)
        {
            List<ExtractedFile> files = new List<ExtractedFile>();

            EmfParser parser = new EmfParser();

            foreach (string key in iconRids.Keys)
            {
                string iconPath = fileRids[key];
                string filePath = fileRids[iconRids[key]];

                ZipArchiveEntry iconEntry = archiveFiles.First(x => x.FullName.EndsWith(StripFirstElement(iconPath)));
                ZipArchiveEntry fileEntry = archiveFiles.First(x => x.FullName.EndsWith(StripFirstElement(filePath)));

                EmfFile emfFile = parser.Parse(iconEntry.GetBytes());

                files.Add(new ExtractedFile(fileEntry)
                {
                    FileName = emfFile.GetTextContent()
                });
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