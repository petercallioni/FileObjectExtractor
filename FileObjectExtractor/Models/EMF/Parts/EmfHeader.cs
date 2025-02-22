namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfHeader : EmfFilePart
    {
        public EmfField Bounds = new(16);
        public EmfField Frame = new(16);
        public EmfField RecordSignature = new(4);
        public EmfField Version = new(4);
        public EmfField Bytes = new(4);
        public EmfField Records = new(4);
        public EmfField Handles = new(2);
        public EmfField Reserved = new(2);
        public EmfField<uint> nDescription = new(4);
        public EmfField<uint> offDescription = new(4);
        public EmfField nPalEntries = new(4);
        public EmfField Device = new(8);
        public EmfField Millimeters = new(8);

        public EmfHeader()
        {
            Fields.AddRange(new[]
              {
                Bounds, Frame, RecordSignature, Version, Bytes, Records, Handles, Reserved, nDescription, offDescription, nPalEntries, Device, Millimeters
            });
        }
    }
}