namespace FileObjectExtractor.Services
{
    public interface IProgressService
    {
        void HideProgress();
        void SetMaximum(int maximum);
        void SetMessage(string message);
        void SetProgress(int progress);
        void ShowProgress();
    }
}