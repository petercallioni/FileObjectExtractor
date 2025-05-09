using FileObjectExtractor.Models.EMF.Enums;

namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfTextRecord : EmfFilePart
    {
        public RecordType RecordType;
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
        public int CharSizeModifer;
        public bool HasRectangle;

        public EmfTextRecord(RecordType recordType, uint size)
        {
            RecordType = recordType;
            Size.Value = size;
            CharSizeModifer = RecordType == RecordType.EMR_EXTTEXTOUTA ? 1 : 2; // EMR_EXTTEXTOUTW = 2 bytes per char
            Fields.AddRange(new[]
            {
               Bounds, iGraphicsMode, exScale, eyScale, Reference, Chars, offString, Options
            });
        }
    }
}