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
using LottieUWP.Value;

namespace LottieUWP.Animation.Keyframe
{
    internal class ValueCallbackKeyframeAnimation<TK, TA> : BaseKeyframeAnimation<TK, TA>
    {
        private readonly LottieFrameInfo<TA> _frameInfo = new LottieFrameInfo<TA>();

        internal ValueCallbackKeyframeAnimation(ILottieValueCallback<TA> valueCallback) : base(new List<Keyframe<TK>>())
        {
            SetValueCallback(valueCallback);
        }

        /// <summary>
        /// If this doesn't return 1, then <see cref="set_Progress"/> will always clamp the progress 
        /// to 0.
        /// </summary>
        protected override float EndProgress => 1f;

        public override void OnValueChanged()
        {
            if (ValueCallback != null)
            {
                base.OnValueChanged();
            }
        }

        public override TA Value => ValueCallback.GetValueInternal(0f, 0f, default(TA), default(TA), Progress, Progress, Progress);

        public override TA GetValue(Keyframe<TK> keyframe, float keyframeProgress)
        {
            return Value;
        }
    }
}