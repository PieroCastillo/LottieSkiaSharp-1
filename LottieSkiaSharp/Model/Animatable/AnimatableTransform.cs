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
using System.Numerics;
using LottieUWP.Animation.Content;
using LottieUWP.Animation.Keyframe;
using LottieUWP.Model.Content;
using LottieUWP.Model.Layer;

namespace LottieUWP.Model.Animatable
{
    public class AnimatableTransform : IModifierContent, IContentModel
    {
        public AnimatableTransform()
            : this(
                new AnimatablePathValue(),
                new AnimatablePathValue(),
                new AnimatableScaleValue(),
                new AnimatableFloatValue(),
                new AnimatableIntegerValue(),
                new AnimatableFloatValue(),
                new AnimatableFloatValue()
            )
        {
        }

        public AnimatableTransform(AnimatablePathValue anchorPoint, IAnimatableValue<Vector2?, Vector2?> position, AnimatableScaleValue scale, AnimatableFloatValue rotation, AnimatableIntegerValue opacity, AnimatableFloatValue startOpacity, AnimatableFloatValue endOpacity)
        {
            AnchorPoint = anchorPoint;
            Position = position;
            Scale = scale;
            Rotation = rotation;
            Opacity = opacity;
            StartOpacity = startOpacity;
            EndOpacity = endOpacity;
        }

        internal AnimatablePathValue AnchorPoint { get; }

        internal IAnimatableValue<Vector2?, Vector2?> Position { get; }

        internal AnimatableScaleValue Scale { get; }

        internal AnimatableFloatValue Rotation { get; }

        internal AnimatableIntegerValue Opacity { get; }

        // Used for repeaters 
        internal AnimatableFloatValue StartOpacity { get; }
        internal AnimatableFloatValue EndOpacity { get; }

        public TransformKeyframeAnimation CreateAnimation()
        {
            return new TransformKeyframeAnimation(this);
        }

        public IContent ToContent(ILottieDrawable drawable, BaseLayer layer)
        {
            return null;
        }
    }
}