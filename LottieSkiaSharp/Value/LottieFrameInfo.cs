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
    /// <summary>
    /// Data class for use with <see cref="LottieValueCallback{T}"/>.
    /// You should* not* hold a reference to the frame info parameter passed to your callback. It will be reused.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LottieFrameInfo<T>
    {
        internal LottieFrameInfo<T> Set(
            float startFrame,
            float endFrame,
            T startValue,
            T endValue,
            float linearKeyframeProgress,
            float interpolatedKeyframeProgress,
            float overallProgress
        )
        {
            StartFrame = startFrame;
            EndFrame = endFrame;
            StartValue = startValue;
            EndValue = endValue;
            LinearKeyframeProgress = linearKeyframeProgress;
            InterpolatedKeyframeProgress = interpolatedKeyframeProgress;
            OverallProgress = overallProgress;
            return this;
        }

        public float StartFrame { get; private set; }

        public float EndFrame { get; private set; }

        public T StartValue { get; private set; }

        public T EndValue { get; private set; }

        public float LinearKeyframeProgress { get; private set; }

        public float InterpolatedKeyframeProgress { get; private set; }

        public float OverallProgress { get; private set; }
    }
}
