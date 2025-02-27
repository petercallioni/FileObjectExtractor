using Avalonia;
using Avalonia.Controls;

namespace FileObjectExtractor.Views;

public partial class ErrorWindow : Window
{

    // To eliminate an IDE warning about not having a default constructor
    public ErrorWindow()
    {
        InitializeComponent();
    }

    public ErrorWindow(Window parent)
    {
        InitializeComponent();
        this.Owner = parent;

#if DEBUG
        this.AttachDevTools();
#endif
    }
}