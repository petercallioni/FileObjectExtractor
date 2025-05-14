using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;
using System;

namespace FileObjectExtractor.ViewModels
{
    public class FileTrustWindowViewModel : ClosableViewModel
    {
        public IRelayCommand DismissCommand { get; init; }
        public IRelayCommand ConfirmCommand { get; init; }
        private readonly Action confirmAction;

        public FileTrustWindowViewModel(IWindowService windowService, Action confirmAction) : base(windowService)
        {
            this.confirmAction = confirmAction;
            DismissCommand = new RelayCommand(Close);
            ConfirmCommand = new RelayCommand(Confirm);
        }

        private void Confirm()
        {
            confirmAction();
            Close();
        }
    }
}