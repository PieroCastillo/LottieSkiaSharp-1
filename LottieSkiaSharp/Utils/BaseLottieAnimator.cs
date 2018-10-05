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

namespace LottieUWP.Utils
{
    public abstract class BaseLottieAnimator : ValueAnimator
    {
        public long StartDelay
        {
            get => throw new Exception("LottieAnimator does not support getStartDelay.");
            set => throw new Exception("LottieAnimator does not support setStartDelay.");
        }

        public override long Duration
        {
            set => throw new Exception("LottieAnimator does not support setDuration.");
        }

        public override IInterpolator Interpolator
        {
            set => throw new Exception("LottieAnimator does not support setInterpolator.");
        }

        public event EventHandler<LottieAnimatorStartEventArgs> AnimationStart;
        public event EventHandler<LottieAnimatorEndEventArgs> AnimationEnd;
        public event EventHandler AnimationCancel;
        public event EventHandler AnimationRepeat;

        public class LottieAnimatorStartEventArgs : EventArgs
        {
            public bool IsReverse { get; }

            public LottieAnimatorStartEventArgs(bool isReverse)
            {
                IsReverse = isReverse;
            }
        }

        public class LottieAnimatorEndEventArgs : EventArgs
        {
            public bool IsReverse { get; }

            public LottieAnimatorEndEventArgs(bool isReverse)
            {
                IsReverse = isReverse;
            }
        }

        public virtual void OnAnimationStart(bool isReverse)
        {
            AnimationStart?.Invoke(this, new LottieAnimatorStartEventArgs(isReverse));
        }

        public virtual void OnAnimationRepeat()
        {
            AnimationRepeat?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnAnimationEnd(bool isReverse)
        {
            AnimationEnd?.Invoke(this, new LottieAnimatorEndEventArgs(isReverse));
        }

        public virtual void OnAnimationCancel()
        {
            AnimationCancel?.Invoke(this, EventArgs.Empty);
        }
    }
}
