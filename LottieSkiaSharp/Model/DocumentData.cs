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
using System;
using SkiaSharp;

namespace LottieUWP.Model
{
    public class DocumentData
    {
        internal readonly string Text;
        internal readonly string FontName;
        internal readonly double Size;
        internal readonly int Justification;
        internal readonly int Tracking;
        internal readonly double LineHeight;
        internal readonly double BaselineShift;
        internal readonly SKColor Color;
        internal readonly SKColor StrokeColor;
        internal readonly double StrokeWidth;
        internal readonly bool StrokeOverFill;

        internal DocumentData(string text, string fontName, double size, int justification, int tracking, double lineHeight, double baselineShift, SKColor color, SKColor strokeColor, double strokeWidth, bool strokeOverFill)
        {
            Text = text;
            FontName = fontName;
            Size = size;
            Justification = justification;
            Tracking = tracking;
            LineHeight = lineHeight;
            BaselineShift = baselineShift;
            Color = color;
            StrokeColor = strokeColor;
            StrokeWidth = strokeWidth;
            StrokeOverFill = strokeOverFill;
        }

        public override int GetHashCode()
        {
            int result;
            long temp;
            result = Text.GetHashCode();
            result = 31 * result + FontName.GetHashCode();
            result = (int)(31 * result + Size);
            result = 31 * result + Justification;
            result = 31 * result + Tracking;
            temp = BitConverter.DoubleToInt64Bits(LineHeight);
            result = 31 * result + (int)(temp ^ ((long)((ulong)temp >> 32)));
            result = 31 * result + Color.GetHashCode();
            return result;
        }
    }
}
