using System.Collections.Generic;
using SkiaSharp;
using LottieUWP.Utils;
using LottieUWP.Value;

namespace LottieUWP.Animation.Keyframe
{
    internal class ColorKeyframeAnimation : KeyframeAnimation<SKColor?>
    {
        internal ColorKeyframeAnimation(List<Keyframe<SKColor?>> keyframes) : base(keyframes)
        {
        }

        public override SKColor? GetValue(Keyframe<SKColor?> keyframe, float keyframeProgress)
        {
            if (keyframe.StartValue == null || keyframe.EndValue == null)
            {
                throw new System.InvalidOperationException("Missing values for keyframe.");
            }
            var startColor = keyframe.StartValue;
            var endColor = keyframe.EndValue;

            if (ValueCallback != null)
            {
                var value = ValueCallback.GetValueInternal(keyframe.StartFrame.Value, keyframe.EndFrame.Value, startColor, endColor, keyframeProgress, LinearCurrentKeyframeProgress, Progress);
                if (value != null)
                {
                    return value;
                }
            }

            return GammaEvaluator.Evaluate(keyframeProgress, startColor.Value, endColor.Value);
        }
    }
}