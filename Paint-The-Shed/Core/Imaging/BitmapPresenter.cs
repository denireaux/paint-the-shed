using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Paint_The_Shed.Core.Imaging;

public sealed class BitmapPresenter : IDisposable
{
    public WriteableBitmap Bitmap { get; private set; }

    private readonly int _width;
    private readonly int _height;
    private readonly byte[] _bgraBytes;

    public BitmapPresenter(int width, int height)
    {
        _width = width;
        _height = height;

        Bitmap = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        _bgraBytes = new byte[width * height * 4];
    }

    // argbPixels: 0xAARRGGBB
    public void BlitFromArgb(int[] argbPixels)
    {
        if (argbPixels.Length != _width * _height)
            throw new ArgumentException("Pixel array size mismatch.", nameof(argbPixels));

        int count = _width * _height;

        // Convert into BGRA byte array
        int bi = 0;
        for (int i = 0; i < count; i++)
        {
            int c = argbPixels[i];

            byte a = (byte)((c >> 24) & 0xFF);
            byte r = (byte)((c >> 16) & 0xFF);
            byte g = (byte)((c >> 8) & 0xFF);
            byte b = (byte)(c & 0xFF);

            _bgraBytes[bi + 0] = b;
            _bgraBytes[bi + 1] = g;
            _bgraBytes[bi + 2] = r;
            _bgraBytes[bi + 3] = a;
            bi += 4;
        }

        using var fb = Bitmap.Lock();
        Marshal.Copy(_bgraBytes, 0, fb.Address, _bgraBytes.Length);
    }

    public void Dispose()
    {
        Bitmap?.Dispose();
        Bitmap = null!;
    }
}