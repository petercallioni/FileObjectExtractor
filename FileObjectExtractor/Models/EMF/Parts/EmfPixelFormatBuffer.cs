namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfPixelFormatBuffer : EmfFilePart
    {
        public EmfField Contents = new(typeof(string)); // Variable Length

        public EmfPixelFormatBuffer()
        {
            Fields.AddRange(new[]
            {
               Contents
            });
        }
    }
}