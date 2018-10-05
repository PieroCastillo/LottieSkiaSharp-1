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
using LottieUWP.Model.Animatable;

namespace LottieUWP.Parser
{
    public static class AnimatableValueParser
    {
        public static AnimatableFloatValue ParseFloat(JsonReader reader, LottieComposition composition)
        {
            return ParseFloat(reader, composition, true);
        }

        public static AnimatableFloatValue ParseFloat(JsonReader reader, LottieComposition composition, bool isDp)
        {
            return new AnimatableFloatValue(Parse(reader, isDp ? Utils.Utils.DpScale() : 1f, composition, FloatParser.Instance));
        }

        internal static AnimatableIntegerValue ParseInteger(JsonReader reader, LottieComposition composition)
        {
            return new AnimatableIntegerValue(Parse(reader, composition, IntegerParser.Instance));
        }

        internal static AnimatablePointValue ParsePoint(JsonReader reader, LottieComposition composition)
        {
            return new AnimatablePointValue(Parse(reader, Utils.Utils.DpScale(), composition, PointFParser.Instance));
        }

        internal static AnimatableScaleValue ParseScale(JsonReader reader, LottieComposition composition)
        {
            return new AnimatableScaleValue(Parse(reader, composition, ScaleXyParser.Instance));
        }

        internal static AnimatableShapeValue ParseShapeData(JsonReader reader, LottieComposition composition)
        {
            return new AnimatableShapeValue(Parse(reader, Utils.Utils.DpScale(), composition, ShapeDataParser.Instance));
        }

        internal static AnimatableTextFrame ParseDocumentData(JsonReader reader, LottieComposition composition)
        {
            return new AnimatableTextFrame(Parse(reader, composition, DocumentDataParser.Instance));
        }

        internal static AnimatableColorValue ParseColor(JsonReader reader, LottieComposition composition)
        {
            return new AnimatableColorValue(Parse(reader, composition, ColorParser.Instance));
        }

        internal static AnimatableGradientColorValue ParseGradientColor(JsonReader reader, LottieComposition composition, int points)
        {
            return new AnimatableGradientColorValue(Parse(reader, composition, new GradientColorParser(points)));
        }

        /// <summary>
        /// Will return null if the animation can't be played such as if it has expressions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="composition"></param>
        /// <param name="valueParser"></param>
        /// <returns></returns>
        private static List<Keyframe<T>> Parse<T>(JsonReader reader, LottieComposition composition, IValueParser<T> valueParser)
        {
            return KeyframesParser.Parse(reader, composition, 1, valueParser);
        }

        /// <summary>
        /// Will return null if the animation can't be played such as if it has expressions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="scale"></param>
        /// <param name="composition"></param>
        /// <param name="valueParser"></param>
        /// <returns></returns>
        private static List<Keyframe<T>> Parse<T>(JsonReader reader, float scale, LottieComposition composition, IValueParser<T> valueParser)
        {
            return KeyframesParser.Parse(reader, composition, scale, valueParser);
        }
    }
}