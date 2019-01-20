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
using LottieUWP.Model;
using LottieUWP.Model.Content;
using LottieUWP.Model.Layer;
using LottieUWP.Utils;
using LottieUWP.Value;
using LottieUWP.Expansion;

namespace LottieUWP.Animation.Content
{
    internal class GradientFillContent : IDrawingContent, IKeyPathElementContent
    {
        /// <summary>
        /// Cache the gradients such that it runs at 30fps.
        /// </summary>
        private const int CacheStepsMs = 32;

        private readonly BaseLayer _layer;
        private readonly bool _hidden;
        private readonly Dictionary<long, SKShader> _linearGradientCache = new Dictionary<long, SKShader>();
        private readonly Dictionary<long, SKShader> _radialGradientCache = new Dictionary<long, SKShader>();
        private readonly Matrix3X3 _shaderMatrix = Matrix3X3.CreateIdentity();
        private readonly SKPath _path = new SKPath();
        private readonly SKPaint _paint = SkRectExpansion.CreateSkPaint();
        //private Rect _boundsRect;
        private readonly List<IPathContent> _paths = new List<IPathContent>();
        private readonly GradientType _type;
        private readonly IBaseKeyframeAnimation<GradientColor, GradientColor> _colorAnimation;
        private readonly IBaseKeyframeAnimation<int?, int?> _opacityAnimation;
        private readonly IBaseKeyframeAnimation<Vector2?, Vector2?> _startPointAnimation;
        private readonly IBaseKeyframeAnimation<Vector2?, Vector2?> _endPointAnimation;
        private IBaseKeyframeAnimation<SKColorFilter, SKColorFilter> _colorFilterAnimation;
        private readonly ILottieDrawable _lottieDrawable;
        private readonly int _cacheSteps;

        internal GradientFillContent(ILottieDrawable lottieDrawable, BaseLayer layer, GradientFill fill)
        {
            _layer = layer;
            Name = fill.Name;
            _hidden = fill.IsHidden;
            _lottieDrawable = lottieDrawable;
            _type = fill.GradientType;
            _path.FillType = fill.FillType;
            _cacheSteps = (int)(lottieDrawable.Composition.Duration / CacheStepsMs);

            _colorAnimation = fill.GradientColor.CreateAnimation();
            _colorAnimation.ValueChanged += OnValueChanged;
            layer.AddAnimation(_colorAnimation);

            _opacityAnimation = fill.Opacity.CreateAnimation();
            _opacityAnimation.ValueChanged += OnValueChanged;
            layer.AddAnimation(_opacityAnimation);

            _startPointAnimation = fill.StartPoint.CreateAnimation();
            _startPointAnimation.ValueChanged += OnValueChanged;
            layer.AddAnimation(_startPointAnimation);

            _endPointAnimation = fill.EndPoint.CreateAnimation();
            _endPointAnimation.ValueChanged += OnValueChanged;
            layer.AddAnimation(_endPointAnimation);
        }

        private void OnValueChanged(object sender, EventArgs eventArgs)
        {
            _lottieDrawable.InvalidateSelf();
        }

        public void SetContents(List<IContent> contentsBefore, List<IContent> contentsAfter)
        {
            for (var i = 0; i < contentsAfter.Count; i++)
            {
                if (contentsAfter[i] is IPathContent pathContent)
                {
                    _paths.Add(pathContent);
                }
            }
        }

        public void Draw(SKCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
            if (_hidden)
            {
                return;
            }
            LottieLog.BeginSection("GradientFillContent.Draw");
            _path.Reset();
            for (var i = 0; i < _paths.Count; i++)
            {
                var m = parentMatrix.ToSKMatrix();
                _path.AddPath(_paths[i].Path, ref m);
            }

            //_path.ComputeBounds(out _boundsRect);

            SKShader shader;
            if (_type == GradientType.Linear)
            {
                shader = LinearGradient;
            }
            else
            {
                shader = RadialGradient;
            }
            _shaderMatrix.Set(parentMatrix);
            shader = SKShader.CreateLocalMatrix(shader,_shaderMatrix.ToSKMatrix());
            _paint.Shader = shader;

            if (_colorFilterAnimation != null)
            {
                _paint.ColorFilter = _colorFilterAnimation.Value;
            }

            var alpha = (byte)(parentAlpha / 255f * _opacityAnimation.Value / 100f * 255);
            _paint.SetAlpha(alpha);

            canvas.DrawPath(_path, _paint);
            LottieLog.EndSection("GradientFillContent.Draw");
        }

        public void GetBounds(out SKRect outBounds, Matrix3X3 parentMatrix)
        {
            _path.Reset();
            for (var i = 0; i < _paths.Count; i++)
            {
                var m = parentMatrix.ToSKMatrix();
                _path.AddPath(_paths[i].Path, ref m);
            }

            _path.GetBounds(out outBounds);
            // Add padding to account for rounding errors.
            RectExt.Set(ref outBounds, outBounds.Left - 1, outBounds.Top - 1, outBounds.Right + 1, outBounds.Bottom + 1);
        }

        public string Name { get; }

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
                gradient = SKShader.CreateLinearGradient(startPoint.Value.ToSkPoint(),endPoint.Value.ToSkPoint(),colors,positions,SKShaderTileMode.Clamp);
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
                var x0 = startPoint.Value.X;
                var y0 = startPoint.Value.Y;
                var x1 = endPoint.Value.X;
                var y1 = endPoint.Value.Y;
                var r = (float)MathExt.Hypot(x1 - x0, y1 - y0);
                gradient = SKShader.CreateRadialGradient(new SKPoint(x0,y0),r,colors,positions,SKShaderTileMode.Clamp);
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

        public void ResolveKeyPath(KeyPath keyPath, int depth, List<KeyPath> accumulator, KeyPath currentPartialKeyPath)
        {
            MiscUtils.ResolveKeyPath(keyPath, depth, accumulator, currentPartialKeyPath, this);
        }

        public void AddValueCallback<T>(LottieProperty property, ILottieValueCallback<T> callback)
        {
            if (property == LottieProperty.ColorFilter)
            {
                if (callback == null)
                {
                    _colorFilterAnimation = null;
                }
                else
                {
                    _colorFilterAnimation = new ValueCallbackKeyframeAnimation<SKColorFilter, SKColorFilter>((ILottieValueCallback<SKColorFilter>)callback);
                    _colorFilterAnimation.ValueChanged += OnValueChanged;
                    _layer.AddAnimation(_colorFilterAnimation);
                }
            }
        }
    }
}