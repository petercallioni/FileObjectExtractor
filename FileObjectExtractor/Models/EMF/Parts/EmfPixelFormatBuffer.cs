namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfPixelFormatBuffer : EmfFilePart
    {
        public EmfField<string> Contents = new(); // Variable Length

        public EmfPixelFormatBuffer()
        {
            Fields.AddRange(new[]
            {
               Contents
            });
        }
    }
}