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
using LottieUWP.Model.Animatable;
using LottieUWP.Model.Content;

namespace LottieUWP.Parser
{
    static class ShapeTrimPathParser
    {
        internal static ShapeTrimPath Parse(JsonReader reader, LottieComposition composition)
        {
            string name = null;
            ShapeTrimPath.Type type = ShapeTrimPath.Type.Simultaneously;
            AnimatableFloatValue start = null;
            AnimatableFloatValue end = null;
            AnimatableFloatValue offset = null;
            bool hidden = false;

            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "s":
                        start = AnimatableValueParser.ParseFloat(reader, composition, false);
                        break;
                    case "hd":
                        hidden = reader.NextBoolean();
                        break;
                    case "e":
                        end = AnimatableValueParser.ParseFloat(reader, composition, false);
                        break;
                    case "o":
                        offset = AnimatableValueParser.ParseFloat(reader, composition, false);
                        break;
                    case "nm":
                        name = reader.NextString();
                        break;
                    case "m":
                        type = (ShapeTrimPath.Type)reader.NextInt();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            return new ShapeTrimPath(name, type, start, end, offset, hidden);
        }
    }
}
