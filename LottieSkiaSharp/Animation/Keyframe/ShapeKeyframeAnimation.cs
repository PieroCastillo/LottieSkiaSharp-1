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
using LottieUWP.Utils;
using LottieUWP.Value;
using SkiaSharp;

namespace LottieUWP.Animation.Keyframe
{
    internal class ShapeKeyframeAnimation : BaseKeyframeAnimation<ShapeData, SKPath>
    {
        private readonly ShapeData _tempShapeData = new ShapeData();
        private readonly SKPath _tempPath = new SKPath();

        internal ShapeKeyframeAnimation(List<Keyframe<ShapeData>> keyframes) : base(keyframes)
        {
        }

        public override SKPath GetValue(Keyframe<ShapeData> keyframe, float keyframeProgress)
        {
            var startShapeData = keyframe.StartValue;
            var endShapeData = keyframe.EndValue;

            _tempShapeData.InterpolateBetween(startShapeData, endShapeData, keyframeProgress);
            MiscUtils.GetPathFromData(_tempShapeData, _tempPath);
            return _tempPath;
        }
    }
}