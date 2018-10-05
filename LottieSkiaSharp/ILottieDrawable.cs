using SkiaSharp;

namespace LottieUWP
{
    public interface ILottieDrawable
    {
        void InvalidateSelf();
        bool UseTextGlyphs();
        SKTypeface GetTypeface(string fontFamily, SKTypefaceStyle style);
        TextDelegate TextDelegate { get; }
        SKBitmap GetImageAsset(string id);
        LottieComposition Composition { get; }
        bool EnableMergePathsForKitKatAndAbove();
    }
}
