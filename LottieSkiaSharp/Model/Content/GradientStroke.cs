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

namespace LottieUWP.Model.Content
{
    public class GradientStroke : IContentModel
    {
        public GradientStroke(string name, GradientType gradientType, AnimatableGradientColorValue gradientColor, AnimatableIntegerValue opacity, AnimatablePointValue startPoint, AnimatablePointValue endPoint, AnimatableFloatValue width, ShapeStroke.LineCapType capType, ShapeStroke.LineJoinType joinType, float miterLimit, List<AnimatableFloatValue> lineDashPattern, AnimatableFloatValue dashOffset, bool hidden)
        {
            Name = name;
            GradientType = gradientType;
            GradientColor = gradientColor;
            Opacity = opacity;
            StartPoint = startPoint;
            EndPoint = endPoint;
            Width = width;
            CapType = capType;
            JoinType = joinType;
            MiterLimit = miterLimit;
            LineDashPattern = lineDashPattern;
            DashOffset = dashOffset;
            IsHidden = hidden;
        }

        internal string Name { get; }

        internal GradientType GradientType { get; }

        internal AnimatableGradientColorValue GradientColor { get; }

        internal AnimatableIntegerValue Opacity { get; }

        internal AnimatablePointValue StartPoint { get; }

        internal AnimatablePointValue EndPoint { get; }

        internal AnimatableFloatValue Width { get; }

        internal ShapeStroke.LineCapType CapType { get; }

        internal ShapeStroke.LineJoinType JoinType { get; }

        internal float MiterLimit { get; }

        internal bool IsHidden { get; }

        internal List<AnimatableFloatValue> LineDashPattern { get; }

        internal AnimatableFloatValue DashOffset { get; }

        public IContent ToContent(ILottieDrawable drawable, BaseLayer layer)
        {
            return new GradientStrokeContent(drawable, layer, this);
        }
    }
}