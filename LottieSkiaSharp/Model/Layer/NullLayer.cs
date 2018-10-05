using LottieUWP.Animation.Content;

namespace LottieUWP.Model.Layer
{
    internal class NullLayer : BaseLayer
    {
        internal NullLayer(ILottieDrawable lottieDrawable, Layer layerModel) : base(lottieDrawable, layerModel)
        {
        }

        public override void DrawLayer(SkiaSharp.SKCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
            // Do nothing.
        }

        public override void GetBounds(out SkiaSharp.SKRect outBounds, Matrix3X3 parentMatrix)
        {
            base.GetBounds(out outBounds, parentMatrix);
            RectExt.Set(ref outBounds, 0, 0, 0, 0);
        }
    }
}