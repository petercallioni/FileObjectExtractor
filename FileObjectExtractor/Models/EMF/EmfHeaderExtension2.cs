namespace FileObjectExtractor.Models.EMF
{
    public class EmfHeaderExtension2 : EmfFilePart
    {
        public EmfField MicrometersX = new(4);
        public EmfField MicrometersY = new(4);

        public EmfHeaderExtension2()
        {
            Fields.AddRange(new[]
            {
               MicrometersX, MicrometersY
            });
        }
    }
}