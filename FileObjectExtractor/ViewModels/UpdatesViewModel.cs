﻿using CommunityToolkit.Mvvm.Input;
using FileObjectExtractor.Services;
using FileObjectExtractor.Updates;
using FileObjectExtractor.ViewModels.Interfaces;
using System;
using System.Threading.Tasks;
using Version = FileObjectExtractor.Updates.Version;

namespace FileObjectExtractor.ViewModels
{
    public class UpdatesViewModel : ClosableViewModel, IUpdatesViewModel
    {
        private readonly IWindowService windowService;
        private readonly IUpdateService updateService;
        private Update? update;
        private bool hasUpdate;
        private string newVersion = string.Empty;
        private string updateTooltip = string.Empty;
        private readonly ProgressIndicatorViewModel progressIndicatorViewModel;
        private readonly IProgressService progressService;

        private readonly Version currentVersion;

        public IAsyncRelayCommand CheckForUpdatesCommand => new AsyncRelayCommand(CheckForUpdatesAsync);
        public IAsyncRelayCommand DownloadAndInstallCommand => new AsyncRelayCommand(DownloadAndInstall);

        public UpdatesViewModel(IWindowService windowService, IUpdateService updateService) : base(windowService)
        {
            this.windowService = windowService;
            this.updateService = updateService;
            this.progressIndicatorViewModel = new ProgressIndicatorViewModel();
            progressService = new ProgressService(progressIndicatorViewModel);
            currentVersion = Utilities.VersionNumber.Version();
        }
        public UpdatesViewModel(IWindowService windowService, IUpdateService updateService, Update? update) : base(windowService)
        {
            this.windowService = windowService;
            this.updateService = updateService;
            this.update = update;
            currentVersion = Utilities.VersionNumber.Version();
            this.progressIndicatorViewModel = new ProgressIndicatorViewModel(1);
            progressService = new ProgressService(progressIndicatorViewModel);

            if (update != null)
            {
                HasUpdate = update.IsUpgrade;
                NewVersion = update.Version.ToString();
            }
        }

        public bool HasUpdate
        {
            get => hasUpdate;
            set
            {
                hasUpdate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UpdateTooltip));
            }
        }
        public string NewVersion
        {
            get => newVersion;
            set
            {
                newVersion = value;
                OnPropertyChanged();
            }
        }

        public string UpdateTooltip
        {
            get => HasUpdate ? $"New version available: {NewVersion}" : "No updates available";
        }

        public Update? Update
        {
            get => update;
            set => update = value;
        }
        public ProgressIndicatorViewModel ProgressIndicatorViewModel => progressIndicatorViewModel;
        public IProgressService ProgressService => progressService;

        public Version CurrentVersion => currentVersion;

        /// <summary>
        /// Checks for available updates in a asynchronous but unsafe manner.
        /// This method should be avoided in favor of CheckForUpdatesAsync.
        /// </summary>
        /// <remarks>
        /// This method does not catch any exceptions.
        /// </remarks>
        public void CheckForUpdatesUnsafe()
        {
            update = updateService.CheckForUpdate();

            if (update != null)
            {
                HasUpdate = update.IsUpgrade;
                NewVersion = update.Version.ToString();
            }
        }

        /// <summary>
        /// Asynchronously checks for available updates using the update service.
        /// </summary>
        public async Task CheckForUpdatesAsync()
        {
            await ExceptionSafeAsync(async () =>
            {
                update = await updateService.CheckForUpdateAsync();
            });

            if (update != null)
            {
                HasUpdate = update.IsUpgrade;
                NewVersion = update.Version.ToString();
            }
        }

        private async Task DownloadAndInstall()
        {
            if (update == null)
            {
                windowService.ShowErrorWindow(new Exception("No update available to download."));
                return;
            }

            progressService.ShowProgress();

            // Create a progress object that updates the view model.
            IProgress<DownloadProgressReport> progress = new Progress<DownloadProgressReport>(report =>
            {
                progressService.SetProgress(report.Fraction);
                progressService.SetMessage(
                    $"{ByteSizeFormatter.Format(report.BytesDownloaded)} of {ByteSizeFormatter.Format(report.TotalBytes)} Downloaded");
            });

            await ExceptionSafeAsync(async () =>
            {
                DownloadedUpdateFiles downloadedFiles = await updateService.DownloadUpdate(update, progress);
                await updateService.InstallUpdate(update, downloadedFiles);
            });
        }
    }
}
