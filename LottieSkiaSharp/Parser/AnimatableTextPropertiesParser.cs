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

namespace LottieUWP.Parser
{
    public static class AnimatableTextPropertiesParser
    {
        public static AnimatableTextProperties Parse(JsonReader reader, LottieComposition composition)
        {
            AnimatableTextProperties anim = null;

            reader.BeginObject();
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "a":
                        anim = ParseAnimatableTextProperties(reader, composition);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();
            if (anim == null)
            {
                // Not sure if this is possible. 
                return new AnimatableTextProperties(null, null, null, null);
            }
            return anim;
        }

        private static AnimatableTextProperties ParseAnimatableTextProperties(JsonReader reader, LottieComposition composition)
        {
            AnimatableColorValue color = null;
            AnimatableColorValue stroke = null;
            AnimatableFloatValue strokeWidth = null;
            AnimatableFloatValue tracking = null;

            reader.BeginObject();
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "fc":
                        color = AnimatableValueParser.ParseColor(reader, composition);
                        break;
                    case "sc":
                        stroke = AnimatableValueParser.ParseColor(reader, composition);
                        break;
                    case "sw":
                        strokeWidth = AnimatableValueParser.ParseFloat(reader, composition);
                        break;
                    case "t":
                        tracking = AnimatableValueParser.ParseFloat(reader, composition);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();

            return new AnimatableTextProperties(color, stroke, strokeWidth, tracking);
        }
    }
}
