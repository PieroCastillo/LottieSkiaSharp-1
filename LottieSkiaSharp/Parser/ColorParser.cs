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
using SkiaSharp;
using Newtonsoft.Json;

namespace LottieUWP.Parser
{
    internal class ColorParser : IValueParser<SKColor?>
    {
        internal static readonly ColorParser Instance = new ColorParser();

        public SKColor? Parse(JsonReader reader, float scale)
        {
            bool isArray = reader.Peek() == JsonToken.StartArray;
            if (isArray)
            {
                reader.BeginArray();
            }
            var r = reader.NextDouble();
            var g = reader.NextDouble();
            var b = reader.NextDouble();
            var a = reader.NextDouble();
            if (isArray)
            {
                reader.EndArray();
            }

            if (r <= 1 && g <= 1 && b <= 1 && a <= 1)
            {
                r *= 255;
                g *= 255;
                b *= 255;
                a *= 255;
            }
            return new SKColor((byte)r, (byte)g, (byte)b, (byte)a);
        }
    }
}
