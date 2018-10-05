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
using LottieUWP.Model;

namespace LottieUWP.Parser
{
    public class DocumentDataParser : IValueParser<DocumentData>
    {
        public static readonly DocumentDataParser Instance = new DocumentDataParser();

        public DocumentData Parse(JsonReader reader, float scale)
        {
            string text = null;
            string fontName = null;
            double size = 0;
            int justification = 0;
            int tracking = 0;
            double lineHeight = 0;
            double baselineShift = 0;
            SKColor fillColor = default;
            SKColor strokeColor = default;
            double strokeWidth = 0;
            bool strokeOverFill = true;

            reader.BeginObject();
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "t":
                        text = reader.NextString();
                        break;
                    case "f":
                        fontName = reader.NextString();
                        break;
                    case "s":
                        size = reader.NextDouble();
                        break;
                    case "j":
                        justification = reader.NextInt();
                        break;
                    case "tr":
                        tracking = reader.NextInt();
                        break;
                    case "lh":
                        lineHeight = reader.NextDouble();
                        break;
                    case "ls":
                        baselineShift = reader.NextDouble();
                        break;
                    case "fc":
                        fillColor = JsonUtils.JsonToColor(reader);
                        break;
                    case "sc":
                        strokeColor = JsonUtils.JsonToColor(reader);
                        break;
                    case "sw":
                        strokeWidth = reader.NextDouble();
                        break;
                    case "of":
                        strokeOverFill = reader.NextBoolean();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();

            return new DocumentData(text, fontName, size, justification, tracking, lineHeight,
                baselineShift, fillColor, strokeColor, strokeWidth, strokeOverFill);
        }
    }
}
