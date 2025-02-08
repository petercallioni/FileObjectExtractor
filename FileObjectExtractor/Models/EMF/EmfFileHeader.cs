namespace FileObjectExtractor.Models.EMF
{
    public class EmfFileHeader : EmfFilePart
    {
        public EmfField Type = new(4, typeof(uint));
        public EmfField Size = new(4, typeof(uint));

        public EmfFileHeader()
        {
            Fields.AddRange(new[]
              {
                Type, Size
            });
        }
    }
}