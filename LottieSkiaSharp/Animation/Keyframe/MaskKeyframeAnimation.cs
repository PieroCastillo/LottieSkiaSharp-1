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
using System.Collections.Generic;
using LottieUWP.Model.Content;
using SkiaSharp;

namespace LottieUWP.Animation.Keyframe
{
    internal class MaskKeyframeAnimation
    {
        private readonly List<IBaseKeyframeAnimation<ShapeData, SKPath>> _maskAnimations;
        private readonly List<IBaseKeyframeAnimation<int?, int?>> _opacityAnimations;

        internal MaskKeyframeAnimation(List<Mask> masks)
        {
            Masks = masks;
            _maskAnimations = new List<IBaseKeyframeAnimation<ShapeData, SKPath>>(masks.Count);
            _opacityAnimations = new List<IBaseKeyframeAnimation<int?, int?>>(masks.Count);
            for (var i = 0; i < masks.Count; i++)
            {
                _maskAnimations.Add(masks[i].MaskPath.CreateAnimation());
                var opacity = masks[i].Opacity;
                _opacityAnimations.Add(opacity.CreateAnimation());
            }
        }

        internal List<Mask> Masks { get; }

        internal List<IBaseKeyframeAnimation<ShapeData, SKPath>> MaskAnimations => _maskAnimations;

        internal List<IBaseKeyframeAnimation<int?, int?>> OpacityAnimations => _opacityAnimations;
    }
}