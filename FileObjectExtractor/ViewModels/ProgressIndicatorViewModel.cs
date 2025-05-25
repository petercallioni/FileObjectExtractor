namespace FileObjectExtractor.ViewModels
{
    public class ProgressIndicatorViewModel : ViewModelBase
    {
        private double progress;
        private double maxProgress;
        private string message;
        private bool showProgress;
        public double Progress
        {
            get => progress; set
            {
                progress = value;
                OnPropertyChanged();
            }
        }
        public string Message
        {
            get => message; set
            {
                message = value;
                OnPropertyChanged();
            }
        }

        public double MaxProgress
        {
            get => maxProgress; set
            {
                maxProgress = value;
                OnPropertyChanged();
            }
        }

        public bool ShowProgress
        {
            get => showProgress; set
            {
                showProgress = value;
                OnPropertyChanged();
            }
        }

        public ProgressIndicatorViewModel() : base(null!)
        {
            progress = 0;
            maxProgress = 100;
            showProgress = false;
            message = "";
        }

        public ProgressIndicatorViewModel(int maxProgress, string initialMessage = "") : base(null!)
        {
            progress = 0;
            this.maxProgress = maxProgress;
            this.showProgress = false;
            message = initialMessage;
        }
    }
}
