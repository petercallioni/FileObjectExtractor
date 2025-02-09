using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FileObjectExtractor.Models;
using FileObjectExtractor.ViewModels;
using FileObjectExtractor.Views;

namespace FileObjectExtractor
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);

                MainWindow window = new MainWindow();
                FileController fileController = new FileController(window);
                MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(fileController);
                window.DataContext = mainWindowViewModel;

                desktop.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}