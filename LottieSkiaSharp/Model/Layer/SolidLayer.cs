//   Copyright 2018 yinyue200.com

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System.Numerics;
using SkiaSharp;
using LottieUWP.Animation.Content;
using LottieUWP.Animation.Keyframe;
using LottieUWP.Value;
using LottieUWP.Expansion;

namespace LottieUWP.Model.Layer
{
    internal class SolidLayer : BaseLayer
    {
        private readonly SKPaint _paint = SkRectExpansion.CreateSkPaintWithoutAntialias();
        private Vector2[] _points = new Vector2[4];
        private readonly SKPath _path = new SKPath();
        private IBaseKeyframeAnimation<SKColorFilter, SKColorFilter> _colorFilterAnimation;

        internal SolidLayer(ILottieDrawable lottieDrawable, Layer layerModel) : base(lottieDrawable, layerModel)
        {
            LayerModel = layerModel;

            _paint.SetAlpha(0);
            _paint.Style = SKPaintStyle.Fill;
            _paint.Color = layerModel.SolidColor;
        }

        public override void DrawLayer(SKCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
            int backgroundAlpha = LayerModel.SolidColor.Alpha;
            if (backgroundAlpha == 0)
            {
                return;
            }

            var alpha = (byte)(parentAlpha / 255f * (backgroundAlpha / 255f * Transform.Opacity.Value / 100f) * 255);
            _paint.SetAlpha(alpha);
            if (_colorFilterAnimation != null)
            {
                _paint.ColorFilter = _colorFilterAnimation.Value;
            }
            if (alpha > 0)
            {
                _points[0] = new Vector2(0, 0);
                _points[1] = new Vector2(LayerModel.SolidWidth, 0);
                _points[2] = new Vector2(LayerModel.SolidWidth, LayerModel.SolidHeight);
                _points[3] = new Vector2(0, LayerModel.SolidHeight);

                // We can't map rect here because if there is rotation on the transform then we aren't 
                // actually drawing a rect. 
                parentMatrix.MapPoints(ref _points);
                _path.Reset();
                _path.MoveTo(_points[0].X, _points[0].Y);
                _path.LineTo(_points[1].X, _points[1].Y);
                _path.LineTo(_points[2].X, _points[2].Y);
                _path.LineTo(_points[3].X, _points[3].Y);
                _path.LineTo(_points[0].X, _points[0].Y);
                _path.Close();
                canvas.DrawPath(_path, _paint);
            }
        }

        public override void GetBounds(out SKRect outBounds, Matrix3X3 parentMatrix)
        {
            base.GetBounds(out outBounds, parentMatrix);
            RectExt.Set(ref Rect, 0, 0, LayerModel.SolidWidth, LayerModel.SolidHeight);
            BoundsMatrix.MapRect(ref Rect);
            RectExt.Set(ref outBounds, Rect);
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
                    _colorFilterAnimation = new ValueCallbackKeyframeAnimation<SKColorFilter, SKColorFilter>((ILottieValueCallback<SKColorFilter>)callback);
                }
            }
        }
    }
}