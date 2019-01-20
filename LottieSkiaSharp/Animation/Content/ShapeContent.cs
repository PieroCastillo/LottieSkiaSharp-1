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
using LottieUWP.Animation.Keyframe;
using LottieUWP.Model.Content;
using LottieUWP.Model.Layer;
using SkiaSharp;

namespace LottieUWP.Animation.Content
{
    internal class ShapeContent : IPathContent
    {
        private SKPath _path = new SKPath();

        private readonly bool _hidden;
        private readonly ILottieDrawable _lottieDrawable;
        private readonly IBaseKeyframeAnimation<ShapeData, SKPath> _shapeAnimation;

        private bool _isPathValid;
        private TrimPathContent _trimPath;

        internal ShapeContent(ILottieDrawable lottieDrawable, BaseLayer layer, ShapePath shape)
        {
            Name = shape.Name;
            _hidden = shape.IdHidden;
            _lottieDrawable = lottieDrawable;
            _shapeAnimation = shape.GetShapePath().CreateAnimation();
            layer.AddAnimation(_shapeAnimation);
            _shapeAnimation.ValueChanged += OnValueChanged;
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
                    // Trim path individually will be handled by the stroke where paths are combined.
                    _trimPath = trimPathContent;
                    _trimPath.ValueChanged += OnValueChanged;
                }
            }
        }

        public SKPath Path
        {
            get
            {
                if (_isPathValid)
                {
                    return _path;
                }

                _path.Reset();

                if (_hidden)
                {
                    _isPathValid = true;
                    return _path;
                }

                _path = _shapeAnimation.Value;
                _path.FillType = SKPathFillType.EvenOdd;

                Utils.Utils.ApplyTrimPathIfNeeded(ref _path, _trimPath);

                _isPathValid = true;
                return _path;
            }
        }

        public string Name { get; }
    }
}