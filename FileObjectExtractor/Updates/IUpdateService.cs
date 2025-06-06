using System;
using System.Threading.Tasks;

namespace FileObjectExtractor.Updates
{
    public interface IUpdateService
    {
        Update CheckForUpdate();
        Task<Update> CheckForUpdateAsync(bool runSynchronously = false);
        Task<DownloadedUpdateFiles> DownloadUpdate(Update update, IProgress<DownloadProgressReport>? progress = null);
        Task InstallUpdate(Update update, DownloadedUpdateFiles files, bool noRestart = false);
    }
}