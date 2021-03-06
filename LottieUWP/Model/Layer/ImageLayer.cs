using System;
using Windows.Foundation;
using LottieUWP.Animation.Content;
using LottieUWP.Animation.Keyframe;
using LottieUWP.Value;
using Microsoft.Graphics.Canvas;

namespace LottieUWP.Model.Layer
{
    internal class ImageLayer : BaseLayer
    {
        private readonly Paint _paint = new Paint(Paint.AntiAliasFlag | Paint.FilterBitmapFlag);
        private Rect _src;
        private Rect _dst;
        private IBaseKeyframeAnimation<ColorFilter, ColorFilter> _colorFilterAnimation;

        internal ImageLayer(ILottieDrawable lottieDrawable, Layer layerModel) : base(lottieDrawable, layerModel)
        {
        }

        public override void DrawLayer(BitmapCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
            var bitmap = Bitmap;
            if (bitmap == null)
            {
                return;
            }
            var density = Utils.Utils.DpScale();

            _paint.Alpha = parentAlpha;
            if (_colorFilterAnimation != null)
            {
                _paint.ColorFilter = _colorFilterAnimation.Value;
            }
            canvas.Save();
            canvas.Concat(parentMatrix);
            RectExt.Set(ref _src, 0, 0, PixelWidth, PixelHeight);
            RectExt.Set(ref _dst, 0, 0, (int)(PixelWidth * density), (int)(PixelHeight * density));
            canvas.DrawBitmap(bitmap, _src, _dst, _paint);
            canvas.Restore();
        }

        public override void GetBounds(out Rect outBounds, Matrix3X3 parentMatrix)
        {
            base.GetBounds(out outBounds, parentMatrix);
            var bitmap = Bitmap;
            if (bitmap != null)
            {
                RectExt.Set(ref outBounds, outBounds.Left, outBounds.Top, Math.Min(outBounds.Right, PixelWidth), Math.Min(outBounds.Bottom, PixelHeight));
                BoundsMatrix.MapRect(ref outBounds);
            }
        }
        private int PixelWidth => (int)Bitmap.SizeInPixels.Width;

        private int PixelHeight => (int)Bitmap.SizeInPixels.Height;

        private CanvasBitmap Bitmap
        {
            get
            {
                var refId = LayerModel.RefId;
                return LottieDrawable.GetImageAsset(refId);
            }
        }

        public override void AddValueCallback<T>(LottieProperty property, ILottieValueCallback<T> callback)
        {
            base.AddValueCallback(property, callback);
            if (property == LottieProperty.ColorFilter)
            {
                if (callback == null)
                {
                    _colorFilterAnimation = null;
                }
                else
                {
                    _colorFilterAnimation = new ValueCallbackKeyframeAnimation<ColorFilter, ColorFilter>((ILottieValueCallback<ColorFilter>)callback);
                }
            }
        }
    }
}