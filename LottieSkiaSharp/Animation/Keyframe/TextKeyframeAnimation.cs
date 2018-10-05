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
using LottieUWP.Model;
using LottieUWP.Value;

namespace LottieUWP.Animation.Keyframe
{
    internal class TextKeyframeAnimation : KeyframeAnimation<DocumentData>
    {
        internal TextKeyframeAnimation(List<Keyframe<DocumentData>> keyframes) : base(keyframes)
        {
        }

        public override DocumentData GetValue(Keyframe<DocumentData> keyframe, float keyframeProgress)
        {
            return keyframe.StartValue;
        }
    }
}
