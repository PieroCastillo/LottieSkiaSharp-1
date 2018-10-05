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
using System.Numerics;

namespace LottieUWP.Animation.Keyframe
{
    internal class PointKeyframeAnimation : KeyframeAnimation<Vector2?>
    {
        private Vector2 _point;

        internal PointKeyframeAnimation(List<Keyframe<Vector2?>> keyframes) : base(keyframes)
        {
        }

        public override Vector2? GetValue(Keyframe<Vector2?> keyframe, float keyframeProgress)
        {
            if (keyframe.StartValue == null || keyframe.EndValue == null)
            {
                throw new System.InvalidOperationException("Missing values for keyframe.");
            }

            var startPoint = keyframe.StartValue;
            var endPoint = keyframe.EndValue;

            if (ValueCallback != null)
            {
                var value = ValueCallback.GetValueInternal(keyframe.StartFrame.Value, keyframe.EndFrame.Value, startPoint, endPoint, keyframeProgress, LinearCurrentKeyframeProgress, Progress);
                if (value != null)
                {
                    return value;
                }
            }

            _point.X = startPoint.Value.X + keyframeProgress * (endPoint.Value.X - startPoint.Value.X);
            _point.Y = startPoint.Value.Y + keyframeProgress * (endPoint.Value.Y - startPoint.Value.Y);

            return _point;
        }
    }
}