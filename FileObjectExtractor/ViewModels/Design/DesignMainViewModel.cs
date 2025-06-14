﻿using FileObjectExtractor.Models;

namespace FileObjectExtractor.ViewModels.Design
{
    public class DesignMainViewModel : MainWindowViewModel
    {
        public DesignMainViewModel() : base(null!, new MockWindowService(), null!, null!)
        {
            // Add mock data here
            InputFile = new InputFileViewModel(new System.Uri("file:///Sample.docx"), OfficeType.EXCEL);

            ProgressIndicator.ShowProgress = true;
            ProgressIndicator.Progress = 60;
            ProgressIndicator.Message = "Current File.docx";

            CanGoBack = true;
            SortOrder = SortOrder.DOCUMENT;

            for (int i = 1; i < 7; i++)
            {
                ExtractedFile extractedFile = new ExtractedFile(new byte[1000 * i]);
                ExtractedFileViewModel extractedFileVM = new ExtractedFileViewModel(null!, extractedFile, null!, null!, null!);
                extractedFileVM.IsSelected = i % 2 != 0;
                extractedFile.OpenableInFox = i % 2 != 0;

                if (i == 3)
                {
                    extractedFileVM.ExtractedFile.FileNameWarnings.Add("Sample Warning 1");
                    extractedFileVM.ExtractedFile.FileNameWarnings.Add("Sample Warning 2");
                    extractedFileVM.ExtractedFile.FileName = $"This is a very long filename that is over 50 characters long.docx";
                    extractedFileVM.CanOpen = true;
                }
                else
                {
                    extractedFileVM.ExtractedFile.FileName = $"Embedded File {i}.docx";
                }

                FilteredExtractedFiles.Add(extractedFileVM);
            }
        }
    }
}
