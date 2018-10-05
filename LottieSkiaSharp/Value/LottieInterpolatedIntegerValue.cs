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
using LottieUWP.Utils;

namespace LottieUWP.Value
{
    // ReSharper disable once UnusedMember.Global
    public class LottieInterpolatedIntegerValue : LottieInterpolatedValue<int>
    {
        public LottieInterpolatedIntegerValue(int startValue, int endValue)
            : base(startValue, endValue)
        {
        }

        public LottieInterpolatedIntegerValue(int startValue, int endValue, IInterpolator interpolator)
            : base(startValue, endValue, interpolator)
        {
        }

        protected override int InterpolateValue(int startValue, int endValue, float progress)
        {
            return MiscUtils.Lerp(startValue, endValue, progress);
        }
    }
}