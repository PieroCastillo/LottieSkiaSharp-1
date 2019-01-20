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
using LottieUWP.Animation.Keyframe;
using LottieUWP.Model;
using LottieUWP.Model.Content;
using LottieUWP.Model.Layer;
using LottieUWP.Utils;
using LottieUWP.Value;
using SkiaSharp;

namespace LottieUWP.Animation.Content
{
    internal class EllipseContent : IPathContent, IKeyPathElementContent
    {
        private const float EllipseControlPointPercentage = 0.55228f;

        private SKPath _path = new SKPath();

        private readonly ILottieDrawable _lottieDrawable;
        private readonly IBaseKeyframeAnimation<Vector2?, Vector2?> _sizeAnimation;
        private readonly IBaseKeyframeAnimation<Vector2?, Vector2?> _positionAnimation;
        private readonly CircleShape _circleShape;

        private TrimPathContent _trimPath;
        private bool _isPathValid;

        internal EllipseContent(ILottieDrawable lottieDrawable, BaseLayer layer, CircleShape circleShape)
        {
            Name = circleShape.Name;
            _lottieDrawable = lottieDrawable;
            _sizeAnimation = circleShape.Size.CreateAnimation();
            _positionAnimation = circleShape.Position.CreateAnimation();
            _circleShape = circleShape;

            layer.AddAnimation(_sizeAnimation);
            layer.AddAnimation(_positionAnimation);

            _sizeAnimation.ValueChanged += OnValueChanged;
            _positionAnimation.ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, EventArgs eventArgs)
        {
            Invalidate();
        }

        private void Invalidate()
        {
            _isPathValid = false;
            _lottieDrawable.InvalidateSelf();
        }

        public void SetContents(List<IContent> contentsBefore, List<IContent> contentsAfter)
        {
            for (var i = 0; i < contentsBefore.Count; i++)
            {
                if (contentsBefore[i] is TrimPathContent trimPathContent && trimPathContent.Type == ShapeTrimPath.Type.Simultaneously)
                {
                    _trimPath = trimPathContent;
                    _trimPath.ValueChanged += OnValueChanged;
                }
            }
        }

        public string Name { get; }

        public SKPath Path
        {
            get
            {
                if (_isPathValid)
                {
                    return _path;
                }

                _path.Reset();

                if (_circleShape.IsHidden)
                {
                    _isPathValid = true;
                    return _path;
                }

                var size = _sizeAnimation.Value;
                var halfWidth = size.Value.X / 2f;
                var halfHeight = size.Value.Y / 2f;
                // TODO: handle bounds

                var cpW = halfWidth * EllipseControlPointPercentage;
                var cpH = halfHeight * EllipseControlPointPercentage;

                _path.Reset();
                if (_circleShape.IsReversed)
                {
                    _path.MoveTo(0, -halfHeight);
                    _path.CubicTo(0 - cpW, -halfHeight, -halfWidth, 0 - cpH, -halfWidth, 0);
                    _path.CubicTo(-halfWidth, 0 + cpH, 0 - cpW, halfHeight, 0, halfHeight);
                    _path.CubicTo(0 + cpW, halfHeight, halfWidth, 0 + cpH, halfWidth, 0);
                    _path.CubicTo(halfWidth, 0 - cpH, 0 + cpW, -halfHeight, 0, -halfHeight);
                }
                else
                {
                    _path.MoveTo(0, -halfHeight);
                    _path.CubicTo(0 + cpW, -halfHeight, halfWidth, 0 - cpH, halfWidth, 0);
                    _path.CubicTo(halfWidth, 0 + cpH, 0 + cpW, halfHeight, 0, halfHeight);
                    _path.CubicTo(0 - cpW, halfHeight, -halfWidth, 0 + cpH, -halfWidth, 0);
                    _path.CubicTo(-halfWidth, 0 - cpH, 0 - cpW, -halfHeight, 0, -halfHeight);
                }

                var position = _positionAnimation.Value;
                _path.Offset(position.Value.X, position.Value.Y);

                _path.Close();

                Utils.Utils.ApplyTrimPathIfNeeded(ref _path, _trimPath);

                _isPathValid = true;
                return _path;
            }
        }

        public void ResolveKeyPath(KeyPath keyPath, int depth, List<KeyPath> accumulator, KeyPath currentPartialKeyPath)
        {
            MiscUtils.ResolveKeyPath(keyPath, depth, accumulator, currentPartialKeyPath, this);
        }

        public void AddValueCallback<T>(LottieProperty property, ILottieValueCallback<T> callback)
        {
            if (property == LottieProperty.EllipseSize)
            {
                _sizeAnimation.SetValueCallback((ILottieValueCallback<Vector2?>)callback);
            }
            else if (property == LottieProperty.Position)
            {
                _positionAnimation.SetValueCallback((ILottieValueCallback<Vector2?>)callback);
            }
        }
    }
}