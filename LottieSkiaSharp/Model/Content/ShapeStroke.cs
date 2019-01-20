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
using LottieUWP.Animation.Content;
using LottieUWP.Model.Animatable;
using LottieUWP.Model.Layer;
using SkiaSharp;

namespace LottieUWP.Model.Content
{
    public class ShapeStroke : IContentModel
    {
        public enum LineCapType
        {
            Butt,
            Round,
            Unknown
        }

        internal static SKStrokeCap LineCapTypeToPaintCap(LineCapType lineCapType)
        {
            switch (lineCapType)
            {
                case LineCapType.Butt:
                    return SKStrokeCap.Butt;
                case LineCapType.Round:
                    return SKStrokeCap.Round;
                case LineCapType.Unknown:
                default:
                    return SKStrokeCap.Square;
            }
        }
        
        public enum LineJoinType
        {
            Miter,
            Round,
            Bevel
        }

        internal static SKStrokeJoin LineJoinTypeToPaintLineJoin(LineJoinType lineJoinType)
        {
            switch (lineJoinType)
            {
                case LineJoinType.Bevel:
                    return SKStrokeJoin.Bevel;
                case LineJoinType.Miter:
                    return SKStrokeJoin.Miter;
                case LineJoinType.Round:
                default:
                    return SKStrokeJoin.Round;
            }
        }

        public ShapeStroke(string name, AnimatableFloatValue offset, List<AnimatableFloatValue> lineDashPattern, AnimatableColorValue color, AnimatableIntegerValue opacity, AnimatableFloatValue width, LineCapType capType, LineJoinType joinType, float miterLimit, bool hidden)
        {
            Name = name;
            DashOffset = offset;
            LineDashPattern = lineDashPattern;
            Color = color;
            Opacity = opacity;
            Width = width;
            CapType = capType;
            JoinType = joinType;
            MiterLimit = miterLimit;
            IsHidden = hidden;
        }

        public IContent ToContent(ILottieDrawable drawable, BaseLayer layer)
        {
            return new StrokeContent(drawable, layer, this);
        }

        internal string Name { get; }

        internal AnimatableColorValue Color { get; }

        internal AnimatableIntegerValue Opacity { get; }

        internal AnimatableFloatValue Width { get; }

        internal List<AnimatableFloatValue> LineDashPattern { get; }

        internal AnimatableFloatValue DashOffset { get; }

        internal LineCapType CapType { get; }

        internal LineJoinType JoinType { get; }

        internal float MiterLimit { get; }

        internal bool IsHidden { get; }
    }
}