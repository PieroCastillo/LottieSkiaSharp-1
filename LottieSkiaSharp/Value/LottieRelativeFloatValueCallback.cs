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
using LottieUWP.Utils;

namespace LottieUWP.Value
{
    /// <summary>
    /// <see cref="Value.LottieValueCallback{T}"/> that provides a value offset from the original animation 
    ///  rather than an absolute value.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LottieRelativeFloatValueCallback : LottieValueCallback<float?>
    {
        public LottieRelativeFloatValueCallback()
        {
        }

        public LottieRelativeFloatValueCallback(float staticValue)
            : base(staticValue)
        {
        }

        public override float? GetValue(LottieFrameInfo<float?> frameInfo)
        {
            float originalValue = MiscUtils.Lerp(
                frameInfo.StartValue.Value,
                frameInfo.EndValue.Value,
                frameInfo.InterpolatedKeyframeProgress
            );
            float offset = GetOffset(frameInfo);
            return originalValue + offset;
        }

        public float GetOffset(LottieFrameInfo<float?> frameInfo)
        {
            if (Value == null)
            {
                throw new ArgumentException("You must provide a static value in the constructor " +
                                                   ", call setValue, or override getValue.");
            }
            return Value.Value;
        }
    }
}
