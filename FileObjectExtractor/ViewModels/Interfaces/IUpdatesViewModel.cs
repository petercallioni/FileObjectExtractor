using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Updates;

namespace FileObjectExtractor.ViewModels.Interfaces
{
    public interface IUpdatesViewModel
    {
        bool HasUpdate { get; set; }
        string NewVersion { get; set; }
        Update? Update { get; }
        string UpdateTooltip { get; }
        IAsyncRelayCommand CheckForUpdatesCommand { get; }
    }
}