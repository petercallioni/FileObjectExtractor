using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace FileObjectExtractor.Views;

public partial class ErrorWindow : Window
{

    private Point initialMousePosition;

    // To eliminate an IDE warning about not having a default constructor
    public ErrorWindow()
    {
        InitializeComponent();
    }

    public ErrorWindow(Window parent)
    {
        InitializeComponent();
        this.Owner = parent;
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private bool mouseDownForWindowMoving = false;
    private PointerPoint _originalPoint;

    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!mouseDownForWindowMoving) return;

        PointerPoint currentPoint = e.GetCurrentPoint(this);
        Position = new PixelPoint(Position.X + (int)(currentPoint.Position.X - _originalPoint.Position.X),
            Position.Y + (int)(currentPoint.Position.Y - _originalPoint.Position.Y));
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (WindowState == WindowState.Maximized || WindowState == WindowState.FullScreen) return;

        mouseDownForWindowMoving = true;
        _originalPoint = e.GetCurrentPoint(this);
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        mouseDownForWindowMoving = false;
    }
}