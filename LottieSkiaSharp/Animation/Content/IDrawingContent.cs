using SkiaSharp;

namespace LottieUWP.Animation.Content
{
    internal interface IDrawingContent : IContent
    {
        void Draw(SKCanvas canvas, Matrix3X3 parentMatrix, byte alpha);
        void GetBounds(out SKRect outBounds, Matrix3X3 parentMatrix);
    }
}