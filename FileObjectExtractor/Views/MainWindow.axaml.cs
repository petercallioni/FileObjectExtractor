using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using FileObjectExtractor.ViewModels;
using System.Collections.Generic;

namespace FileObjectExtractor.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;
        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif

            AddHandler(DragDrop.DropEvent, Drop);
            AddHandler(DragDrop.DragOverEvent, DragOver);
        }

        private void DragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = DragDropEffects.Copy;
        }

        private void Drop(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                IEnumerable<IStorageItem>? files = e.Data.GetFiles();
                if (files != null)
                {
                    foreach (IStorageItem file in files)
                    {
                        ViewModel.SelectFile(file.Path);
                    }
                }
            }
        }
    }
}