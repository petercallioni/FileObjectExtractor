namespace MicrosoftObjectExtractor.Models.EMF
{
    public class EmfDescriptionBuffer : EmfFilePart
    {
        public EmfField Contents = new(typeof(string)); // Variable Length

        public EmfDescriptionBuffer()
        {
            Fields.AddRange(new[]
            {
               Contents
            });
        }
    }
}