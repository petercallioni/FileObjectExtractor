using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;
using System;

namespace FileObjectExtractor.ViewModels
{
    public class ErrorWindowViewModel : ClosableViewModel
    {
        private Exception exception;
        private string displayText;
        private bool toggle = true;
        private string title;

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

        public string Title { get => title; set => title = value; }

        public ErrorWindowViewModel(Exception exception, IWindowService windowService) : base(windowService)
        {
            this.exception = exception;

            this.title = GetWindowTitle(exception);
            displayText = exception.Message;
            DismissCommand = new RelayCommand(Close);
            ToggleShowMoreCommand = new RelayCommand(ToggleShowMore);
        }

        private void ToggleShowMore()
        {
            toggle = !toggle;
            DisplayText = (toggle ? exception.Message : $"{exception.Message} {exception?.StackTrace?.Trim()}") ?? "Unavaliable";
        }

        private string GetWindowTitle(Exception ex)
        {
            return ex switch
            {
                ArgumentException => "Invalid Argument",
                NotImplementedException => "Not Implemented",
                InvalidOperationException => "Invalid Operation",
                NotSupportedException => "Not Supported",
                NullReferenceException => "Null Reference",
                IndexOutOfRangeException => "Index Out of Range",
                FormatException => "Invalid Format",
                OverflowException => "Overflow",
                DivideByZeroException => "Divide By Zero",
                TimeoutException => "Timeout",
                _ => "Error",
            };
        }
    }
}