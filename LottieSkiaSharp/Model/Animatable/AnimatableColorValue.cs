using System.Collections.Generic;
using SkiaSharp;
using LottieUWP.Value;
using LottieUWP.Animation.Keyframe;

namespace LottieUWP.Model.Animatable
{
    public class AnimatableColorValue : BaseAnimatableValue<SKColor?, SKColor?>
    {
        public AnimatableColorValue(List<Keyframe<SKColor?>> keyframes) : base(keyframes)
        {
        }

        public override IBaseKeyframeAnimation<SKColor?, SKColor?> CreateAnimation()
        {
            return new ColorKeyframeAnimation(Keyframes);
        }
    }
}