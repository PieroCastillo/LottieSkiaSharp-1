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
using LottieUWP.Model.Animatable;
using LottieUWP.Model.Layer;

namespace LottieUWP.Model.Content
{
    public class RectangleShape : IContentModel
    {
        public RectangleShape(string name, IAnimatableValue<Vector2?, Vector2?> position, AnimatablePointValue size, AnimatableFloatValue cornerRadius, bool hidden)
        {
            Name = name;
            Position = position;
            Size = size;
            CornerRadius = cornerRadius;
            IsHidden = hidden;
        }

        internal string Name { get; }

        internal AnimatableFloatValue CornerRadius { get; }

        internal AnimatablePointValue Size { get; }

        internal IAnimatableValue<Vector2?, Vector2?> Position { get; }

        internal bool IsHidden { get; }

        public IContent ToContent(ILottieDrawable drawable, BaseLayer layer)
        {
            return new RectangleContent(drawable, layer, this);
        }

        public override string ToString()
        {
            return "RectangleShape{position=" + Position + ", size=" + Size + '}';
        }
    }
}