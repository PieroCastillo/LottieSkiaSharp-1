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
using System.Collections.ObjectModel;
using System.Numerics;
using LottieUWP.Expansion;

namespace LottieUWP
{
    public static class MatrixExt
    {
        public static Matrix3X3 PreConcat(Matrix3X3 matrix, Matrix3X3 transformAnimationMatrix)
        {
            return matrix * transformAnimationMatrix;
        }

        public static Matrix3X3 PreTranslate(Matrix3X3 matrix, float dx, float dy)
        {
            return matrix * GetTranslate(dx, dy);
        }

        private static Matrix3X3 GetTranslate(float dx, float dy)
        {
            return new Matrix3X3
            {
                M11 = 1,
                M13 = dx,
                M22 = 1,
                M23 = dy,
                M33 = 1
            };
        }

        public static Matrix3X3 PreRotate(Matrix3X3 matrix, float rotation)
        {
            var angle = MathExt.ToRadians(rotation);
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            return matrix * GetRotate(sin, cos);
        }

        public static Matrix3X3 PreRotate(Matrix3X3 matrix, float rotation, float px, float py)
        {
            var angle = MathExt.ToRadians(rotation);
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            var tmp = GetTranslate(-px, -py) * GetRotate(sin, cos) * GetTranslate(px, py);

            return matrix * tmp;
        }

        private static Matrix3X3 GetRotate(double sin, double cos)
        {
            return new Matrix3X3
            {
                M11 = (float)cos,
                M12 = (float)-sin,
                M21 = (float)sin,
                M22 = (float)cos,
                M33 = 1
            };
        }

        public static Matrix3X3 PreScale(Matrix3X3 matrix, float scaleX, float scaleY)
        {
            return matrix * GetScale(scaleX, scaleY);
        }

        public static Matrix3X3 PreScale(Matrix3X3 matrix, float scaleX, float scaleY, float px, float py)
        {
            var tmp = GetTranslate(-px, -py) * GetScale(scaleX, scaleY) * GetTranslate(px, py);

            return matrix * tmp;
        }

        private static Matrix3X3 GetScale(float scaleX, float scaleY)
        {
            return new Matrix3X3
            {
                M11 = scaleX,
                M22 = scaleY,
                M33 = 1
            };
        }

        public static void MapRect(this Matrix3X3 matrix, ref SkiaSharp.SKRect rect)
        {
            var p1 = new Vector2((float)rect.Left, (float)rect.Top);
            var p2 = new Vector2((float)rect.Right, (float)rect.Top);
            var p3 = new Vector2((float)rect.Left, (float)rect.Bottom);
            var p4 = new Vector2((float)rect.Right, (float)rect.Bottom);

            p1 = matrix.Transform(p1);
            p2 = matrix.Transform(p2);
            p3 = matrix.Transform(p3);
            p4 = matrix.Transform(p4);

            var xMin = Math.Min(Math.Min(Math.Min(p1.X, p2.X), p3.X), p4.X);
            var xMax = Math.Max(Math.Max(Math.Max(p1.X, p2.X), p3.X), p4.X);
            var yMax = Math.Max(Math.Max(Math.Max(p1.Y, p2.Y), p3.Y), p4.Y);
            var yMin = Math.Min(Math.Min(Math.Min(p1.Y, p2.Y), p3.Y), p4.Y);

            RectExt.Set(ref rect, SkRectExpansion.CreateSkRect(new SkiaSharp.SKPoint(xMin, yMax), new SkiaSharp.SKPoint(xMax, yMin)));
        }

        public static void MapPoints(this Matrix3X3 matrix, ref Vector2[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = matrix.Transform(points[i]);
            }
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>
            (this IEnumerable<T> source, int size)
        {
            T[] array = null;
            var count = 0;
            foreach (var item in source)
            {
                if (array == null)
                {
                    array = new T[size];
                }
                array[count] = item;
                count++;
                if (count == size)
                {
                    yield return new ReadOnlyCollection<T>(array);
                    array = null;
                    count = 0;
                }
            }
            if (array != null)
            {
                Array.Resize(ref array, count);
                yield return new ReadOnlyCollection<T>(array);
            }
        }
    }
}