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
using LottieUWP.Value;
using System.Collections.Generic;

namespace LottieUWP.Animation.Keyframe
{
    internal class FloatKeyframeAnimation : KeyframeAnimation<float?>
    {
        internal FloatKeyframeAnimation(List<Keyframe<float?>> keyframes) : base(keyframes)
        {
        }

        public override float? GetValue(Keyframe<float?> keyframe, float keyframeProgress)
        {
            if (keyframe.StartValue == null || keyframe.EndValue == null)
            {
                throw new System.InvalidOperationException("Missing values for keyframe.");
            }

            if (ValueCallback != null)
            {
                var value = ValueCallback.GetValueInternal(keyframe.StartFrame.Value, keyframe.EndFrame.Value, keyframe.StartValue, keyframe.EndValue, keyframeProgress, LinearCurrentKeyframeProgress, Progress);
                if (value != null)
                {
                    return value;
                }
            }

            return MathExt.Lerp(keyframe.StartValue.Value, keyframe.EndValue.Value, keyframeProgress);
        }
    }
}