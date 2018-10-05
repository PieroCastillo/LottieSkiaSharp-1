using SkiaSharp;
namespace LottieUWP.Animation.Content
{
    internal interface IPathContent : IContent
    {
        SKPath Path { get; }
    }
}