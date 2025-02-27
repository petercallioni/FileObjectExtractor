using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;
using System;

namespace FileObjectExtractor.ViewModels
{
    public class ErrorWindowViewModel : ViewModelBase
    {
        private Exception exception;
        private readonly IWindowService windowService;
        private string displayText;
        private bool toggle = true;

        public Exception Exception { get => exception; set => exception = value; }
        public IRelayCommand DismissCommand { get; init; }
        public IRelayCommand ToggleShowMoreCommand { get; init; }
        public string DisplayText
        {
            get => displayText; set
            {
                displayText = value;
                OnPropertyChanged();
            }
        }

        public ErrorWindowViewModel(Exception exception, IWindowService windowService)
        {
            this.exception = exception;
            this.windowService = windowService;
            displayText = exception.Message;

            DismissCommand = new RelayCommand(Dismiss);
            ToggleShowMoreCommand = new RelayCommand(ToggleShowMore);
        }

        private void ToggleShowMore()
        {
            toggle = !toggle;
            DisplayText = (toggle ? exception.Message : $"{exception.Message} {exception?.StackTrace?.Trim()}") ?? "Unavaliable";
        }

        private void Dismiss()
        {
            windowService.CloseWindow();
        }
    }
}