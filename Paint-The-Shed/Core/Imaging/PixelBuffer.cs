using System;

namespace Paint_The_Shed.Core.Imaging;

public sealed class PixelBuffer
{
    public int Width { get; }
    public int Height { get; }
    public int[] Pixels { get; }

    public PixelBuffer(int width, int height, int fillColor = unchecked((int)0xFFFFFFFF))
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
        Pixels = new int[width * height];
        Clear(fillColor);
    }

    public void Clear(int color)
    {
        Array.Fill(Pixels, color);
    }

    public void SetPixel(int x, int y, int argb)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height) return;
        Pixels[y * Width + x] = argb;
    }

    public int GetPixel(int x, int y)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height) return 0;
        return Pixels[y * Width + x];
    }

    public void DrawFilledCircle(int cx, int cy, int radius, int argb)
    {
        if (radius <= 0)
        {
            SetPixel(cx, cy, argb);
            return;
        }

        int r2 = radius * radius;
        int minX = cx - radius;
        int maxX = cx + radius;
        int minY = cy - radius;
        int maxY = cy + radius;

        for (int y = minY; y <= maxY; y++)
        {
            int dy = y - cy;
            int dy2 = dy * dy;

            for (int x = minX; x <= maxX; x++)
            {
                int dx = x - cx;
                if (dx * dx + dy2 <= r2)
                    SetPixel(x, y, argb);
            }
        }
    }
}
