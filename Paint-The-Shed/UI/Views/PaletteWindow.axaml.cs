using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Paint_The_Shed.UI.Controls;

namespace Paint_The_Shed.UI.Views;

public partial class PaletteWindow : Window
{
    private readonly PixelCanvasControl _canvas;

    public PaletteWindow(PixelCanvasControl canvas, Window owner)
    {
        InitializeComponent();

        _canvas = canvas;

        Owner = owner;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        StyleSwatches();
        ApplyPreview();
    }

    private void StyleSwatches()
    {
        var swatches = this.FindControl<WrapPanel>("Swatches");
        if (swatches is null) return;

        foreach (var child in swatches.Children)
        {
            if (child is Button b && b.Tag is string hex && Color.TryParse(hex, out var c))
            {
                b.Background = new SolidColorBrush(c);
            }
        }
    }

    private void ApplyPreview()
    {
        var preview = this.FindControl<Border>("Preview");
        if (preview is null) return;

        preview.Background = new SolidColorBrush(PixelCanvasControl.FromArgb(_canvas.MarkerColorArgb));
    }

    private void Swatch_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button b || b.Tag is not string hex) return;
        if (!Color.TryParse(hex, out var c)) return;

        _canvas.MarkerColorArgb = PixelCanvasControl.ToArgb(c);
        ApplyPreview();
    }
}