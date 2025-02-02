using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using MicrosoftObjectExtractor.ViewModels;
using System.Collections.Generic;

namespace MicrosoftObjectExtractor.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;
        public MainWindow()
        {
            InitializeComponent();
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
                        ViewModel.SelectFile(file.Path.ToString());
                    }
                }
            }

        }
    }
}