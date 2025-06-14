﻿using Avalonia.Platform.Storage;
using FileObjectExtractor.Updates;
using System;
using System.Threading.Tasks;

namespace FileObjectExtractor.Services
{
    public interface IWindowService
    {
        void CloseWindow();
        Task<IStorageFile?> OpenFileAsync(string title);
        Task<IStorageFile?> SaveFileAsync(string title, string suggestedFileName);
        Task<string?> SelectFolderAsync(string title);
        void ShowAboutWindow();
        void ShowErrorWindow(Exception ex);
        void ShowFileTrustWindow(Action confirmAction);
        void ShowUpdatesWindow(IUpdateService updateService);
        void ShowUpdatesWindow(IUpdateService updateService, Update? update);
    }
}
