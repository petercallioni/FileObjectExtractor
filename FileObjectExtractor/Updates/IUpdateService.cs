using System;
using System.Threading.Tasks;

namespace FileObjectExtractor.Updates
{
    public interface IUpdateService
    {
        Task<Update> CheckForUpdate();
        Task<DownloadedUpdateFiles> DownloadUpdate(Update update, IProgress<DownloadProgressReport>? progress = null);
        Task InstallUpdate(Update update, DownloadedUpdateFiles files);
    }
}