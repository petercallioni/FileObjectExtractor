namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfHeaderExtension1 : EmfFilePart
    {
        public EmfField<uint> cbPixelFormat = new(4);
        public EmfField<uint> offPixelFormat = new(4);
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