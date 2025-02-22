namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfFileHeader : EmfFilePart
    {
        public EmfField<uint> Type = new(4);
        public EmfField<uint> Size = new(4);

        public EmfFileHeader()
        {
            Fields.AddRange(new[]
              {
                Type, Size
            });
        }
    }
}