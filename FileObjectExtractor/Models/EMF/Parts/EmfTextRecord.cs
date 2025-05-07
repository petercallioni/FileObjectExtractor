namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfTextRecord : EmfFilePart
    {
        public EmfField Size = new(4);
        public EmfField Bounds = new(16);
        public EmfField iGraphicsMode = new(4);
        public EmfField exScale = new(4);
        public EmfField eyScale = new(4);
        public EmfField Reference = new(8);

        public EmfField<uint> Chars = new(4);
        public EmfField<uint> offString = new(4);
        public EmfField<uint> Options = new(4);
        public EmfField Rectangle = new(16);
        public EmfField<uint> offDx = new(4);
        public EmfField<string> OutputString = new();
        public EmfField OutputDx = new();

        public EmfTextRecord()
        {
            Fields.AddRange(new[]
            {
               Size, Bounds, iGraphicsMode, exScale, eyScale, Reference, Chars, offString, Options, Rectangle, offDx
            });
        }
    }
}