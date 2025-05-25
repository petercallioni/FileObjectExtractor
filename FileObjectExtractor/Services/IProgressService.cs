namespace FileObjectExtractor.Services
{
    public interface IProgressService
    {
        void HideProgress();
        void SetMaximum(double maximum);
        void SetMessage(string message);
        void SetProgress(double progress);
        void ShowProgress();
    }
}