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
using LottieUWP.Model.Content;

namespace LottieUWP.Model
{
    public class FontCharacter
    {
        internal static int HashFor(char character, string fontFamily, SkiaSharp.SKTypefaceStyle style)
        {
            var result = 0;
            result = 31 * result + character;
            result = 31 * result + fontFamily.GetHashCode();
            result = 31 * result + style.GetHashCode();
            return result;
        }

        private readonly char _character;
        private readonly string _fontFamily;

        public FontCharacter(List<ShapeGroup> shapes, char character, double size, double width, SkiaSharp.SKTypefaceStyle style, string fontFamily)
        {
            Shapes = shapes;
            _character = character;
            _size = size;
            Width = width;
            _style = style;
            _fontFamily = fontFamily;
        }

        public List<ShapeGroup> Shapes { get; }

        private double _size;

        public double Width { get; }

        private readonly SkiaSharp.SKTypefaceStyle _style;

        public override int GetHashCode()
        {
            return HashFor(_character, _fontFamily, _style);
        }
    }
}
