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
namespace LottieUWP.Value
{
    public abstract class LottieInterpolatedValue<T> : LottieValueCallback<T>
    {
        private readonly T _startValue;
        private readonly T _endValue;
        private readonly IInterpolator _interpolator;

        protected LottieInterpolatedValue(T startValue, T endValue)
            : this(startValue, endValue, new LinearInterpolator())
        {
        }

        protected LottieInterpolatedValue(T startValue, T endValue, IInterpolator interpolator)
        {
            _startValue = startValue;
            _endValue = endValue;
            _interpolator = interpolator;
        }

        public override T GetValue(LottieFrameInfo<T> frameInfo)
        {
            float progress = _interpolator.GetInterpolation(frameInfo.OverallProgress);
            return InterpolateValue(_startValue, _endValue, progress);
        }

        protected abstract T InterpolateValue(T startValue, T endValue, float progress);
    }
}
