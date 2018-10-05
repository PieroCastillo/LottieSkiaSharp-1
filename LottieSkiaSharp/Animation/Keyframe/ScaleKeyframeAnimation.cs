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
using LottieUWP.Value;

namespace LottieUWP.Animation.Keyframe
{
    internal class ScaleKeyframeAnimation : KeyframeAnimation<ScaleXy>
    {
        internal ScaleKeyframeAnimation(List<Keyframe<ScaleXy>> keyframes) : base(keyframes)
        {
        }

        public override ScaleXy GetValue(Keyframe<ScaleXy> keyframe, float keyframeProgress)
        {
            if (keyframe.StartValue == null || keyframe.EndValue == null)
            {
                throw new InvalidOperationException("Missing values for keyframe.");
            }
            var startTransform = keyframe.StartValue;
            var endTransform = keyframe.EndValue;

            if (ValueCallback != null)
            {
                var value = ValueCallback.GetValueInternal(keyframe.StartFrame.Value, keyframe.EndFrame.Value,
                    startTransform, endTransform,
                    keyframeProgress, LinearCurrentKeyframeProgress, Progress);
                if (value != null)
                {
                    return value;
                }
            }

            return new ScaleXy(MathExt.Lerp(startTransform.ScaleX, endTransform.ScaleX, keyframeProgress), MathExt.Lerp(startTransform.ScaleY, endTransform.ScaleY, keyframeProgress));
        }
    }
}