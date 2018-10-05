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
using LottieUWP.Value;
using Newtonsoft.Json;

namespace LottieUWP.Parser
{
    static class KeyframesParser
    {
        internal static List<Keyframe<T>> Parse<T>(JsonReader reader, LottieComposition composition, float scale, IValueParser<T> valueParser)
        {
            List<Keyframe<T>> keyframes = new List<Keyframe<T>>();

            if (reader.Peek() == JsonToken.String)
            {
                composition.AddWarning("Lottie doesn't support expressions.");
                return keyframes;
            }

            reader.BeginObject();
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "k":
                        if (reader.Peek() == JsonToken.StartArray)
                        {
                            reader.BeginArray();

                            if (reader.Peek() == JsonToken.Integer || reader.Peek() == JsonToken.Float)
                            {
                                // For properties in which the static value is an array of numbers. 
                                keyframes.Add(KeyframeParser.Parse(reader, composition, scale, valueParser, false));
                            }
                            else
                            {
                                while (reader.HasNext())
                                {
                                    keyframes.Add(KeyframeParser.Parse(reader, composition, scale, valueParser, true));
                                }
                            }
                            reader.EndArray();
                        }
                        else
                        {
                            keyframes.Add(KeyframeParser.Parse(reader, composition, scale, valueParser, false));
                        }
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();

            SetEndFrames<Keyframe<T>, T>(keyframes);
            return keyframes;
        }

        /// <summary>
        /// The json doesn't include end frames. The data can be taken from the start frame of the next 
        /// keyframe though.
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="keyframes"></param>
        public static void SetEndFrames<TU, TV>(List<TU> keyframes) where TU : Keyframe<TV>
        {
            int size = keyframes.Count;
            for (int i = 0; i < size - 1; i++)
            {
                // In the json, the keyframes only contain their starting frame. 
                keyframes[i].EndFrame = keyframes[i + 1].StartFrame;
            }
            var lastKeyframe = keyframes[size - 1];
            if (lastKeyframe.StartValue == null)
            {
                // The only purpose the last keyframe has is to provide the end frame of the previous 
                // keyframe. 
                keyframes.Remove(lastKeyframe);
            }
        }
    }
}
