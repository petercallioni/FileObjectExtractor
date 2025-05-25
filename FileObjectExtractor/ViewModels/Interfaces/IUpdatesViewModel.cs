using FileObjectExtractor.Updates;

namespace FileObjectExtractor.ViewModels.Interfaces
{
    public interface IUpdatesViewModel
    {
        bool HasUpdate { get; set; }
        string NewVersion { get; set; }
        Update? Update { get; }
    }
}