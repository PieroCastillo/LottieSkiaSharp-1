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
using System;
using System.Collections.Generic;
using System.Numerics;
using SkiaSharp;
using LottieUWP.Animation.Keyframe;
using LottieUWP.Model.Content;
using LottieUWP.Model.Layer;

namespace LottieUWP.Animation.Content
{
    public class GradientStrokeContent : BaseStrokeContent
    {
        /// <summary>
        /// Cache the gradients such that it runs at 30fps.
        /// </summary>
        private const int CacheStepsMs = 32;

        private readonly bool _hidden;
        private readonly Dictionary<long, SKShader> _linearGradientCache = new Dictionary<long, SKShader>();
        private readonly Dictionary<long, SKShader> _radialGradientCache = new Dictionary<long, SKShader>();
        private SKRect _boundsRect;

        private readonly GradientType _type;
        private readonly int _cacheSteps;
        private readonly IBaseKeyframeAnimation<GradientColor, GradientColor> _colorAnimation;
        private readonly IBaseKeyframeAnimation<Vector2?, Vector2?> _startPointAnimation;
        private readonly IBaseKeyframeAnimation<Vector2?, Vector2?> _endPointAnimation;

        internal GradientStrokeContent(ILottieDrawable lottieDrawable, BaseLayer layer, GradientStroke stroke) 
            : base(lottieDrawable, layer, ShapeStroke.LineCapTypeToPaintCap(stroke.CapType), ShapeStroke.LineJoinTypeToPaintLineJoin(stroke.JoinType), stroke.MiterLimit, stroke.Opacity, stroke.Width, stroke.LineDashPattern, stroke.DashOffset)
        {
            Name = stroke.Name;
            _type = stroke.GradientType;
            _hidden = stroke.IsHidden;
            _cacheSteps = (int)(lottieDrawable.Composition.Duration / CacheStepsMs);

            _colorAnimation = stroke.GradientColor.CreateAnimation();
            _colorAnimation.ValueChanged += OnValueChanged;
            layer.AddAnimation(_colorAnimation);

            _startPointAnimation = stroke.StartPoint.CreateAnimation();
            _startPointAnimation.ValueChanged += OnValueChanged;
            layer.AddAnimation(_startPointAnimation);

            _endPointAnimation = stroke.EndPoint.CreateAnimation();
            _endPointAnimation.ValueChanged += OnValueChanged;
            layer.AddAnimation(_endPointAnimation);
        }

        public override void Draw(SKCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
            if (_hidden)
            {
                return;
            }
            GetBounds(out _boundsRect, parentMatrix);
            if (_type == GradientType.Linear)
            {
                Paint.Shader = LinearGradient;
            }
            else
            {
                Paint.Shader = RadialGradient;
            }

            base.Draw(canvas, parentMatrix, parentAlpha);
        }

        public override string Name { get; }

        private SKShader LinearGradient
        {
            get
            {
                var gradientHash = GradientHash;
                if (_linearGradientCache.TryGetValue(gradientHash, out SKShader gradient))
                {
                    return gradient;
                }
                var startPoint = _startPointAnimation.Value;
                var endPoint = _endPointAnimation.Value;
                var gradientColor = _colorAnimation.Value;
                var colors = gradientColor.Colors;
                var positions = gradientColor.Positions;
                var x0 = (int)(_boundsRect.Left + _boundsRect.Width / 2 + startPoint.Value.X);
                var y0 = (int)(_boundsRect.Top + _boundsRect.Height / 2 + startPoint.Value.Y);
                var x1 = (int)(_boundsRect.Left + _boundsRect.Width / 2 + endPoint.Value.X);
                var y1 = (int)(_boundsRect.Top + _boundsRect.Height / 2 + endPoint.Value.Y);
                gradient = SKShader.CreateLinearGradient(new SKPoint(x0, y0), new SKPoint(x1, y1), colors, positions, SKShaderTileMode.Clamp);
                _linearGradientCache.Add(gradientHash, gradient);
                return gradient;
            }
        }

        private SKShader RadialGradient
        {
            get
            {
                var gradientHash = GradientHash;
                if (_radialGradientCache.TryGetValue(gradientHash, out SKShader gradient))
                {
                    return gradient;
                }
                var startPoint = _startPointAnimation.Value;
                var endPoint = _endPointAnimation.Value;
                var gradientColor = _colorAnimation.Value;
                var colors = gradientColor.Colors;
                var positions = gradientColor.Positions;
                var x0 = (int)(_boundsRect.Left + _boundsRect.Width / 2 + startPoint.Value.X);
                var y0 = (int)(_boundsRect.Top + _boundsRect.Height / 2 + startPoint.Value.Y);
                var x1 = (int)(_boundsRect.Left + _boundsRect.Width / 2 + endPoint.Value.X);
                var y1 = (int)(_boundsRect.Top + _boundsRect.Height / 2 + endPoint.Value.Y);
                var r = (float)MathExt.Hypot(x1 - x0, y1 - y0);
                gradient = SKShader.CreateRadialGradient(new SKPoint( x0, y0), r, colors, positions,SKShaderTileMode.Clamp);
                _radialGradientCache.Add(gradientHash, gradient);
                return gradient;
            }
        }

        private int GradientHash
        {
            get
            {
                var startPointProgress = (int)Math.Round(_startPointAnimation.Progress * _cacheSteps);
                var endPointProgress = (int)Math.Round(_endPointAnimation.Progress * _cacheSteps);
                var colorProgress = (int)Math.Round(_colorAnimation.Progress * _cacheSteps);
                var hash = 17;
                if (startPointProgress != 0)
                    hash = hash * 31 * startPointProgress;
                if (endPointProgress != 0)
                    hash = hash * 31 * endPointProgress;
                if (colorProgress != 0)
                    hash = hash * 31 * colorProgress;
                return hash;
            }
        }
    }
}