using FileObjectExtractor.Updates;
using System;

namespace FileObjectExtractor.ViewModels.Design
{
    public class DesignUpdatesViewModel : UpdatesViewModel
    {
        public DesignUpdatesViewModel() : base(null!, null!)
        {
            Update = new Update(
                "https://github.com/petercallioni/FileObjectExtractor/releases/download/v1.1.2/FileObjectExtractor-win-x64.zip",
                "https://github.com/petercallioni/FileObjectExtractor/releases/download/v1.1.2/checksums.txts",
                "v1.2.3",
                UpdateOS.WINDOWS,
                76546048L,
                DateTime.Now
                );

            HasUpdate = true;
            ProgressIndicatorViewModel.ShowProgress = true;
            ProgressIndicatorViewModel.Progress = .6;
            ProgressIndicatorViewModel.MaxProgress = 1;
            ProgressIndicatorViewModel.Message = $"{ByteSizeFormatter.Format(12345678)} of {ByteSizeFormatter.Format(23456789)} Downloaded";

            // NewVersion = "v1.2.3";
        }
    }
}
