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
using LottieUWP.Animation.Content;
using LottieUWP.Model.Animatable;
using LottieUWP.Model.Layer;
using SkiaSharp;

namespace LottieUWP.Model.Content
{
    public class ShapeFill : IContentModel
    {
        private readonly bool _fillEnabled;

        public ShapeFill(string name, bool fillEnabled, SKPathFillType fillType, AnimatableColorValue color, AnimatableIntegerValue opacity, bool hidden)
        {
            Name = name;
            _fillEnabled = fillEnabled;
            FillType = fillType;
            Color = color;
            Opacity = opacity;
            IsHidden = hidden;
        }

        internal string Name { get; }

        internal AnimatableColorValue Color { get; }

        internal AnimatableIntegerValue Opacity { get; }

        internal SKPathFillType FillType { get; }

        internal bool IsHidden { get; }

        public IContent ToContent(ILottieDrawable drawable, BaseLayer layer)
        {
            return new FillContent(drawable, layer, this);
        }

        public override string ToString()
        {
            return "ShapeFill{" + "color=" + ", fillEnabled=" + _fillEnabled + '}';
        }
    }
}