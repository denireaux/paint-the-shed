using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Paint_The_Shed.Core.Imaging;

namespace Paint_The_Shed.UI.Controls;

public sealed class PixelCanvasControl : Control
{
    public static readonly StyledProperty<int> CanvasWidthProperty =
        AvaloniaProperty.Register<PixelCanvasControl, int>(nameof(CanvasWidth), 800);

    public static readonly StyledProperty<int> CanvasHeightProperty =
        AvaloniaProperty.Register<PixelCanvasControl, int>(nameof(CanvasHeight), 600);

    public int CanvasWidth
    {
        get => GetValue(CanvasWidthProperty);
        set => SetValue(CanvasWidthProperty, value);
    }

    public int CanvasHeight
    {
        get => GetValue(CanvasHeightProperty);
        set => SetValue(CanvasHeightProperty, value);
    }

    public int MarkerColorArgb
    {
        get => _brushColor;
        set => _brushColor = value;
    }

    public static int ToArgb(Color c) =>
        (c.A << 24) | (c.R << 16) | (c.G << 8) | c.B;

    public static Color FromArgb(int argb) =>
        Color.FromArgb(
            (byte)((argb >> 24) & 0xFF),
            (byte)((argb >> 16) & 0xFF),
            (byte)((argb >> 8) & 0xFF),
            (byte)(argb & 0xFF));

    private PixelBuffer? _buffer;
    private BitmapPresenter? _presenter;

    private bool _isDrawing;

    // NOTE: The below will set the brush radius and brush color.
    // TODO: Will ultimately need to make this changeable from the desktop application.
    private int _brushRadius = 3;
    private int _brushColor = unchecked((int)0xFF000000);

    public PixelCanvasControl()
    {
        Focusable = true;

        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
        PointerMoved += OnPointerMoved;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsureCanvas();
    }

    private void EnsureCanvas()
    {
        if (_buffer != null && _presenter != null) return;

        _buffer = new PixelBuffer(CanvasWidth, CanvasHeight, unchecked((int)0xFFFFFFFF));
        _presenter = new BitmapPresenter(CanvasWidth, CanvasHeight);

        _presenter.BlitFromArgb(_buffer.Pixels);
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        EnsureCanvas();

        if (_buffer is null || _presenter?.Bitmap is null)
            return;

        var dest = new Rect(0, 0, Bounds.Width, Bounds.Height);
        var source = new Rect(0, 0, _buffer.Width, _buffer.Height);

        context.DrawImage(_presenter.Bitmap, source, dest);
    }

    private void DrawAt(Point p)
    {
        if (_buffer is null || _presenter is null)
            return;

        if (Bounds.Width <= 0 || Bounds.Height <= 0)
            return;

        double sx = _buffer.Width / Bounds.Width;
        double sy = _buffer.Height / Bounds.Height;

        int x = (int)(p.X * sx);
        int y = (int)(p.Y * sy);

        _buffer.DrawFilledCircle(x, y, _brushRadius, _brushColor);

        _presenter.BlitFromArgb(_buffer.Pixels);
        InvalidateVisual();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Focus();

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDrawing = true;
            DrawAt(e.GetPosition(this));
            e.Handled = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDrawing = false;
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDrawing) return;

        var pt = e.GetCurrentPoint(this);
        if (pt.Properties.IsLeftButtonPressed)
        {
            DrawAt(pt.Position);
            e.Handled = true;
        }
    }
}
