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
using System.Diagnostics;
using System.Numerics;
using LottieUWP.Model.Animatable;
using LottieUWP.Value;
using Newtonsoft.Json;

namespace LottieUWP.Parser
{
    public static class AnimatableTransformParser
    {
        public static AnimatableTransform Parse(JsonReader reader, LottieComposition composition)
        {
            AnimatablePathValue anchorPoint = null;
            IAnimatableValue<Vector2?, Vector2?> position = null;
            AnimatableScaleValue scale = null;
            AnimatableFloatValue rotation = null;
            AnimatableIntegerValue opacity = null;
            AnimatableFloatValue startOpacity = null;
            AnimatableFloatValue endOpacity = null;

            bool isObject = reader.Peek() == JsonToken.StartObject;
            if (isObject)
            {
                reader.BeginObject();
            }
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "a":
                        reader.BeginObject();
                        while (reader.HasNext())
                        {
                            if (reader.NextName().Equals("k"))
                            {
                                anchorPoint = AnimatablePathValueParser.Parse(reader, composition);
                            }
                            else
                            {
                                reader.SkipValue();
                            }
                        }
                        reader.EndObject();
                        break;
                    case "p":
                        position = AnimatablePathValueParser.ParseSplitPath(reader, composition);
                        break;
                    case "s":
                        scale = AnimatableValueParser.ParseScale(reader, composition);
                        break;
                    case "rz":
                        composition.AddWarning("Lottie doesn't support 3D layers.");
                        rotation = AnimatableValueParser.ParseFloat(reader, composition, false);
                        break;
                    case "r":
                        rotation = AnimatableValueParser.ParseFloat(reader, composition, false);
                        break;
                    case "o":
                        opacity = AnimatableValueParser.ParseInteger(reader, composition);
                        break;
                    case "so":
                        startOpacity = AnimatableValueParser.ParseFloat(reader, composition, false);
                        break;
                    case "eo":
                        endOpacity = AnimatableValueParser.ParseFloat(reader, composition, false);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            if (isObject)
            {
                reader.EndObject();
            }

            if (anchorPoint == null)
            {
                // Cameras don't have an anchor point property. Although we don't support them, at least 
                // we won't crash. 
                Debug.WriteLine("Layer has no transform property. You may be using an unsupported layer type such as a camera.", LottieLog.Tag);
                anchorPoint = new AnimatablePathValue();
            }

            if (scale == null)
            {
                // Somehow some community animations don't have scale in the transform. 
                scale = new AnimatableScaleValue(new ScaleXy(1f, 1f));
            }

            if (opacity == null)
            {
                // Repeaters have start/end opacity instead of opacity 
                opacity = new AnimatableIntegerValue();
            }

            return new AnimatableTransform(
                anchorPoint, position, scale, rotation, opacity, startOpacity, endOpacity);
        }
    }
}
