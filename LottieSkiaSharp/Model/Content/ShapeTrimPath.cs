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

namespace LottieUWP.Model.Content
{
    public class ShapeTrimPath : IContentModel
    {
        public enum Type
        {
            Simultaneously = 1,
            Individually = 2
        }

        private readonly Type _type;
        private readonly AnimatableFloatValue _start;
        private readonly AnimatableFloatValue _end;
        private readonly AnimatableFloatValue _offset;

        public ShapeTrimPath(string name, Type type, AnimatableFloatValue start, AnimatableFloatValue end, AnimatableFloatValue offset)
        {
            Name = name;
            _type = type;
            _start = start;
            _end = end;
            _offset = offset;
        }

        internal string Name { get; }

        internal new Type GetType()
        {
            return _type;
        }

        internal AnimatableFloatValue End => _end;

        internal AnimatableFloatValue Start => _start;

        internal AnimatableFloatValue Offset => _offset;

        public IContent ToContent(ILottieDrawable drawable, BaseLayer layer)
        {
            return new TrimPathContent(layer, this);
        }

        public override string ToString()
        {
            return "Trim Path: {start: " + _start + ", end: " + _end + ", offset: " + _offset + "}";
        }
    }
}