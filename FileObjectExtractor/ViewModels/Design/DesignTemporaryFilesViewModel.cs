namespace FileObjectExtractor.ViewModels.Design
{
    public class DesignTemporaryFilesViewModel : TemporaryFilesViewModel
    {
        public DesignTemporaryFilesViewModel() : base(null!)
        {
            FileSize = "1.2 MB";
            FileCount = 5;
        }
    }
}
