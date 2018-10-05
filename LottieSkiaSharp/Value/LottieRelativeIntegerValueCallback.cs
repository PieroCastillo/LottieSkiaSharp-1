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
    public class LottieRelativeIntegerValueCallback : LottieValueCallback<int?>
    {
        public override int? GetValue(LottieFrameInfo<int?> frameInfo)
        {
            int originalValue = MiscUtils.Lerp(
                frameInfo.StartValue.Value,
                frameInfo.EndValue.Value,
                frameInfo.InterpolatedKeyframeProgress
            );
            int newValue = GetOffset(frameInfo);
            return originalValue + newValue;
        }

        /// <summary>
        /// Override this to provide your own offset on every frame.
        /// </summary>
        /// <param name="frameInfo"></param>
        /// <returns></returns>
        public int GetOffset(LottieFrameInfo<int?> frameInfo)
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
