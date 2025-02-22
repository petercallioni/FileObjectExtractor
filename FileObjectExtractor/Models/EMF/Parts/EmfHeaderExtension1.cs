namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfHeaderExtension1 : EmfFilePart
    {
        public EmfField cbPixelFormat = new(4, typeof(uint));
        public EmfField offPixelFormat = new(4, typeof(uint));
        public EmfField bOpenGl = new(4);

        public EmfHeaderExtension1()
        {
            Fields.AddRange(new[]
            {
               cbPixelFormat, offPixelFormat, bOpenGl
            });
        }
    }
}