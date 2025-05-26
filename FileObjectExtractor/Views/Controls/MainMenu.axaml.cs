using Avalonia;
using Avalonia.Controls;

namespace FileObjectExtractor.Views.Controls;

public partial class MainMenu : UserControl
{
    public MainMenu()
    {
        InitializeComponent();
        this.AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        // Give focus to this control so it can process Alt key events.
        // Needed as the menu bar is a sub control and does not receive focus initially.
        this.Focus();
    }

    private void Binding(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }
}