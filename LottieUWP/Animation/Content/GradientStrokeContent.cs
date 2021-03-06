using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
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
        private readonly Dictionary<long, LinearGradient> _linearGradientCache = new Dictionary<long, LinearGradient>();
        private readonly Dictionary<long, RadialGradient> _radialGradientCache = new Dictionary<long, RadialGradient>();
        private Rect _boundsRect;

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

        public override void Draw(BitmapCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
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

        private LinearGradient LinearGradient
        {
            get
            {
                var gradientHash = GradientHash;
                if (_linearGradientCache.TryGetValue(gradientHash, out LinearGradient gradient))
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
                gradient = new LinearGradient(x0, y0, x1, y1, colors, positions);
                _linearGradientCache.Add(gradientHash, gradient);
                return gradient;
            }
        }

        private RadialGradient RadialGradient
        {
            get
            {
                var gradientHash = GradientHash;
                if (_radialGradientCache.TryGetValue(gradientHash, out RadialGradient gradient))
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
                gradient = new RadialGradient(x0, y0, r, colors, positions);
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