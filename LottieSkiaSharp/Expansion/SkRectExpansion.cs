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
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SkiaSharp;

namespace LottieUWP.Expansion
{
    static class SkRectExpansion
    {
        public static SKRect CreateSkRect(SKPoint left,SKPoint right)
        {
            var x = Math.Min(left.X, right.Y);
            var y = Math.Min(left.Y, right.Y);

            //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
            var width = Math.Max(Math.Max(left.X, right.X) - x, 0);
            var height = Math.Max(Math.Max(left.Y, right.Y) - y, 0);

            return SKRect.Create(x, y, width, height);
        }
        public static SKMatrix ToSKMatrix(this Matrix3X3 matrix3X3)
        {
            var m = new float[9];
            m[0] = matrix3X3.M11;
            m[1] = matrix3X3.M12;
            m[2] = matrix3X3.M13;
            m[3] = matrix3X3.M21;
            m[4] = matrix3X3.M22;
            m[5] = matrix3X3.M23;
            m[6] = matrix3X3.M31;
            m[7] = matrix3X3.M32;
            m[8] = matrix3X3.M33;
            return new SKMatrix() { Values = m };
        }
        public static Matrix3X3 To3x3Matrix(this SKMatrix matrix3X3)
        {
            var m = new Matrix3X3
            {
                M11 = matrix3X3.Values[0],
                M12 = matrix3X3.Values[1],
                M13 = matrix3X3.Values[2],
                M21 = matrix3X3.Values[3],
                M22 = matrix3X3.Values[4],
                M23 = matrix3X3.Values[5],
                M31 = matrix3X3.Values[6],
                M32 = matrix3X3.Values[7],
                M33 = matrix3X3.Values[8]
            };
            return m;
        }
        public static SKPoint ToSkPoint(this System.Numerics.Vector2 vector2) => new SKPoint(vector2.X, vector2.Y);
        public static void SetAlpha(this SKPaint paint,byte A)
        {
            paint.Color = paint.Color.WithAlpha(A);
        }
        public static SKPaint CreateSkPaint()
        {
            return new SKPaint() { IsAntialias = true, Color = SKColors.Transparent };
        }
        public static SKPaint CreateSkPaintWithoutAntialias()
        {
            return new SKPaint() { Color = SKColors.Transparent};
        }
        public static SKPaint CreateSkPaintWithFilterBitmapFlag()
        {
            return new SKPaint() { Color = SKColors.Transparent,IsAntialias=true };//TODO:ImageFilter=new SKImageFilter()
        }
        public static void ArcTo(this SKPath path,float x, float y, SKRect rect, float startAngle, float sweepAngle)
        {

            var _a = (float)(rect.Width / 2);
            var _b = (float)(rect.Height / 2);
            SKPoint GetPointAtAngle(float t)
            {
                var u = Math.Tan(MathExt.ToRadians(t) / 2);

                var u2 = u * u;

                var _x = _a * (1 - u2) / (u2 + 1);
                var _y = 2 * _b * u / (u2 + 1);

                return new SKPoint((float)(rect.Left + _a + _x), (float)(rect.Top + _b + _y));
            }
            var _endPoint = GetPointAtAngle(startAngle + sweepAngle);
            path.ArcTo(_endPoint, (float)MathExt.ToRadians(sweepAngle), SKPathArcSize.Small, SKPathDirection.Clockwise, new SKPoint(_a, _b));
        }
        public static void Set(this SKPath sKPath,SKPath path)
        {
            sKPath.Reset();
            sKPath.FillType = path.FillType;
            sKPath.AddPath(path);
        }
    }
}
