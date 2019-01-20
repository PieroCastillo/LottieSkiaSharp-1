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
using SkiaSharp;
using LottieUWP.Animation.Keyframe;
using LottieUWP.Model;
using LottieUWP.Model.Content;
using LottieUWP.Model.Layer;
using LottieUWP.Value;

namespace LottieUWP.Animation.Content
{
    internal class StrokeContent : BaseStrokeContent
    {
        private readonly BaseLayer _layer;
        private readonly bool _hidden;
        private readonly IBaseKeyframeAnimation<SKColor?, SKColor?> _colorAnimation;
        private IBaseKeyframeAnimation<SKColorFilter, SKColorFilter> _colorFilterAnimation;

        internal StrokeContent(ILottieDrawable lottieDrawable, BaseLayer layer, ShapeStroke stroke)
            : base(lottieDrawable, layer, ShapeStroke.LineCapTypeToPaintCap(stroke.CapType), ShapeStroke.LineJoinTypeToPaintLineJoin(stroke.JoinType), stroke.MiterLimit, stroke.Opacity, stroke.Width, stroke.LineDashPattern, stroke.DashOffset)
        {
            _layer = layer;
            Name = stroke.Name;
            _hidden = stroke.IsHidden;
            _colorAnimation = stroke.Color.CreateAnimation();
            _colorAnimation.ValueChanged += OnValueChanged;
            layer.AddAnimation(_colorAnimation);
        }

        public override void Draw(SKCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
            if (_hidden)
            {
                return;
            }
            Paint.Color = _colorAnimation.Value ?? SKColors.White;
            if (_colorFilterAnimation != null)
            {
                Paint.ColorFilter = _colorFilterAnimation.Value;
            }
            base.Draw(canvas, parentMatrix, parentAlpha);
        }

        public override string Name { get; }

        public override void AddValueCallback<T>(LottieProperty property, ILottieValueCallback<T> callback)
        {
            base.AddValueCallback(property, callback);
            if (property == LottieProperty.StrokeColor)
            {
                _colorAnimation.SetValueCallback((ILottieValueCallback<SKColor?>)callback);
            }
            else if (property == LottieProperty.ColorFilter)
            {
                if (callback == null)
                {
                    _colorFilterAnimation = null;
                }
                else
                {
                    _colorFilterAnimation = new ValueCallbackKeyframeAnimation<SKColorFilter, SKColorFilter>((ILottieValueCallback<SKColorFilter>)callback);
                    _colorFilterAnimation.ValueChanged += OnValueChanged;
                    _layer.AddAnimation(_colorAnimation);
                }
            }
        }
    }
}