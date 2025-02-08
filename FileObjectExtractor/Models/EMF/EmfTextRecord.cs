namespace FileObjectExtractor.Models.EMF
{
    public class EmfTextRecord : EmfFilePart
    {
        public EmfField Bounds = new(16);
        public EmfField iGraphicsMode = new(4);
        public EmfField exScale = new(4);
        public EmfField eyScale = new(4);

        public EmfField Reference = new(8);
        public EmfField Unknown = new(4);
        public EmfField Chars = new(4, typeof(uint));
        public EmfField offString = new(4, typeof(uint));
        public EmfField Options = new(4, typeof(uint));
        public EmfField Rectangle = new(16);
        public EmfField offDx = new(4, typeof(uint));
        public EmfField OutputString = new(typeof(string));
        public EmfField OutputDx = new();

        public EmfTextRecord()
        {
            Fields.AddRange(new[]
            {
               Bounds, iGraphicsMode, exScale, eyScale, Reference, Unknown, Chars, offString, Options, Rectangle, offDx
            });
        }
    }
}