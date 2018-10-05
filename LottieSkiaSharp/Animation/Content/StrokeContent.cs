﻿using SkiaSharp;
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
        private readonly IBaseKeyframeAnimation<SKColor?, SKColor?> _colorAnimation;
        private IBaseKeyframeAnimation<SKColorFilter, SKColorFilter> _colorFilterAnimation;

        internal StrokeContent(ILottieDrawable lottieDrawable, BaseLayer layer, ShapeStroke stroke) 
            : base(lottieDrawable, layer, ShapeStroke.LineCapTypeToPaintCap(stroke.CapType), ShapeStroke.LineJoinTypeToPaintLineJoin(stroke.JoinType), stroke.MiterLimit, stroke.Opacity, stroke.Width, stroke.LineDashPattern, stroke.DashOffset)
        {
            _layer = layer;
            Name = stroke.Name;
            _colorAnimation = stroke.Color.CreateAnimation();
            _colorAnimation.ValueChanged += OnValueChanged;
            layer.AddAnimation(_colorAnimation);
        }

        public override void Draw(SKCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
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