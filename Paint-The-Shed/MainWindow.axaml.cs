using Avalonia.Controls;
using Avalonia.Interactivity;
using Paint_The_Shed.UI.Views;

namespace Paint_The_Shed;

public partial class MainWindow : Window
{
    private PaletteWindow? _palette;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenPalette_Click(object? sender, RoutedEventArgs e)
    {
        if (_palette is null)
        {
            _palette = new PaletteWindow(Canvas, this);
            _palette.Closed += (_, __) => _palette = null;
        }

        _palette.Show();
        _palette.Activate();
    }
}