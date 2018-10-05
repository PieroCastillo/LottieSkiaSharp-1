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
using SkiaSharp;
using System.Numerics;

namespace LottieUWP.Animation.Keyframe
{
    public class PathKeyframe : Keyframe<Vector2?>
    {
        public PathKeyframe(LottieComposition composition, Keyframe<Vector2?> keyframe)
            : base(composition, keyframe.StartValue, keyframe.EndValue, keyframe.Interpolator, keyframe.StartFrame, keyframe.EndFrame)
        {
            var equals = EndValue != null && StartValue != null && StartValue.Equals(EndValue.Value);
            if (EndValue != null && !equals)
            {
                Path = Utils.Utils.CreatePath(StartValue.Value, EndValue.Value, keyframe.PathCp1, keyframe.PathCp2);
            }
        }

        /// <summary>
        /// This will be null if the startValue and endValue are the same.
        /// </summary>
        internal SKPath Path { get; }
    }
}