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
using System.Threading;

namespace LottieUWP
{
    public abstract class ValueAnimator : Animator, IDisposable
    {
        protected ValueAnimator()
        {
            _interpolator = new AccelerateDecelerateInterpolator();
        }

        public class ValueAnimatorUpdateEventArgs : EventArgs
        {
            public ValueAnimator Animation { get; }

            public ValueAnimatorUpdateEventArgs(ValueAnimator animation)
            {
                Animation = animation;
            }
        }

        public event EventHandler ValueChanged;
        public event EventHandler<ValueAnimatorUpdateEventArgs> Update;

        public void RemoveAllUpdateListeners()
        {
            Update = null;
        }

        public void RemoveAllListeners()
        {
            ValueChanged = null;
        }

        private IInterpolator _interpolator;
        private Timer _timer;

        public abstract float FrameRate { get; set; }

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public int RepeatCount { get; set; }
        public virtual RepeatMode RepeatMode { get; set; }

        public override bool IsRunning => _timer != null;

        public virtual IInterpolator Interpolator
        {
            get => _interpolator;
            set
            {
                if (value == null)
                    value = new LinearInterpolator();
                _interpolator = value;
            }
        }

        public abstract float AnimatedFraction { get; }

        protected void OnAnimationUpdate()
        {
            Update?.Invoke(this, new ValueAnimatorUpdateEventArgs(this));
        }

        protected void PrivateStart()
        {
            if (_timer == null)
            {
                _timer = new Timer(TimerCallback, null, TimeSpan.Zero, GetTimerInterval());
            }
        }

        protected void UpdateTimerInterval()
        {
            _timer?.Change(TimeSpan.Zero, GetTimerInterval());
        }

        private TimeSpan GetTimerInterval()
        {
            System.Diagnostics.Debug.WriteLine("rate:" + FrameRate);
            return TimeSpan.FromTicks((long)Math.Floor(TimeSpan.TicksPerSecond / (decimal)FrameRate));
        }

        protected virtual void RemoveFrameCallback()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private void TimerCallback(object state)
        {
            //System.Diagnostics.Debug.WriteLine("Tick");
            DoFrame();
        }

        public virtual void DoFrame()
        {
            OnValueChanged();
        }

        protected long SystemnanoTime()
        {
            long nano = 10000L * DateTime.Now.Ticks;
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }

        private void Dispose(bool disposing)
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ValueAnimator()
        {
            Dispose(false);
        }
    }
}