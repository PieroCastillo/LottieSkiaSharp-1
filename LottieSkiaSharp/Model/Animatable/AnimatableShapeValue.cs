using System.Collections.Generic;
using LottieUWP.Value;
using LottieUWP.Animation.Keyframe;
using LottieUWP.Model.Content;
using SkiaSharp;

namespace LottieUWP.Model.Animatable
{
    public class AnimatableShapeValue : BaseAnimatableValue<ShapeData, SKPath>
    {
        public AnimatableShapeValue(List<Keyframe<ShapeData>> keyframes) : base(keyframes)
        {
        }

        public override IBaseKeyframeAnimation<ShapeData, SKPath> CreateAnimation()
        {
            return new ShapeKeyframeAnimation(Keyframes);
        }
    }
}