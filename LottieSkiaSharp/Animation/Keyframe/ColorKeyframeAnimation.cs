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
using SkiaSharp;
using LottieUWP.Utils;
using LottieUWP.Value;

namespace LottieUWP.Animation.Keyframe
{
    internal class ColorKeyframeAnimation : KeyframeAnimation<SKColor?>
    {
        internal ColorKeyframeAnimation(List<Keyframe<SKColor?>> keyframes) : base(keyframes)
        {
        }

        public override SKColor? GetValue(Keyframe<SKColor?> keyframe, float keyframeProgress)
        {
            if (keyframe.StartValue == null || keyframe.EndValue == null)
            {
                throw new System.InvalidOperationException("Missing values for keyframe.");
            }
            var startColor = keyframe.StartValue;
            var endColor = keyframe.EndValue;

            if (ValueCallback != null)
            {
                var value = ValueCallback.GetValueInternal(keyframe.StartFrame.Value, keyframe.EndFrame.Value, startColor, endColor, keyframeProgress, LinearCurrentKeyframeProgress, Progress);
                if (value != null)
                {
                    return value;
                }
            }

            return GammaEvaluator.Evaluate(keyframeProgress, startColor.Value, endColor.Value);
        }
    }
}