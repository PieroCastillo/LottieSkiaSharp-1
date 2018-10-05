using System;
using SkiaSharp;

namespace LottieUWP
{
    public static class RectExt
    {
        public static void Set(ref SKRect rect, float left, float top, float right, float bottom)
        {
            rect.Left = left;
            rect.Top = top;
            rect.Size = new SKSize(Math.Abs(right - left), Math.Abs(bottom - top));
        }

        public static void Set(ref SKRect rect, SKRect newRect)
        {
            rect.Left = newRect.Left;
            rect.Top = newRect.Top;
            rect.Size = new SKSize( newRect.Width, newRect.Height);
        }
    }
}
