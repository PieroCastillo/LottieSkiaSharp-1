using System;
using System.Numerics;
using Windows.Graphics.Display;
using Windows.UI;
using LottieUWP.Animation.Content;

namespace LottieUWP.Utils
{
    public static class Utils
    {
        public const int SecondInNanos = 1000000000;
        private static readonly float Sqrt2 = (float)Math.Sqrt(2);
        private static float _dpScale = -1;
        private static float _dpi = -1;

        static Utils()
        {
            DpScale();
            Dpi();
        }

        internal static Path CreatePath(Vector2 startPoint, Vector2 endPoint, Vector2? cp1, Vector2? cp2)
        {
            var path = new Path();
            path.MoveTo(startPoint.X, startPoint.Y);

            if (cp1.HasValue && cp2.HasValue && (cp1.Value.LengthSquared() != 0 || cp2.Value.LengthSquared() != 0))
            {
                path.CubicTo(startPoint.X + cp1.Value.X, startPoint.Y + cp1.Value.Y, endPoint.X + cp2.Value.X, endPoint.Y + cp2.Value.Y, endPoint.X, endPoint.Y);
            }
            else
            {
                path.LineTo(endPoint.X, endPoint.Y);
            }
            return path;
        }

        public static void CloseQuietly(this IDisposable closeable)
        {
            if (closeable != null)
            {
                try
                {
                    closeable.Dispose();
                }
                //catch (RuntimeException rethrown)
                //{
                //    throw rethrown;
                //}
                catch (Exception)
                {
                    // Really quietly
                }
            }
        }

        internal static float GetScale(Matrix3X3 matrix)
        {
            var points = new Vector2[2];
            points[0].X = 0;
            points[0].Y = 0;
            points[1].X = Sqrt2;
            points[1].Y = Sqrt2;
            // Use sqrt(2) so that the hypotenuse is of length 1.
            matrix.MapPoints(ref points);
            var dx = points[1].X - points[0].X;
            var dy = points[1].Y - points[0].Y;

            // TODO: figure out why the result needs to be divided by 2.
            return (float)MathExt.Hypot(dx, dy) / 2f;
        }

        internal static void ApplyTrimPathIfNeeded(Path path, TrimPathContent trimPath)
        {
            if (trimPath == null || trimPath.IsHidden)
            {
                return;
            }
            ApplyTrimPathIfNeeded(path, trimPath.Start.Value.Value / 100f, trimPath.End.Value.Value / 100f, trimPath.Offset.Value.Value / 360f);
        }

        internal static void ApplyTrimPathIfNeeded(Path path, float startValue, float endValue, float offsetValue)
        {
            LottieLog.BeginSection("applyTrimPathIfNeeded");
            Path tempPath = null;
            using (var pathMeasure = new PathMeasure(path))
            {
                var length = pathMeasure.Length;
                if (startValue == 1f && endValue == 0f)
                {
                    LottieLog.EndSection("applyTrimPathIfNeeded");
                    return;
                }
                if (length < 1f || Math.Abs(endValue - startValue - 1) < .01)
                {
                    LottieLog.EndSection("applyTrimPathIfNeeded");
                    return;
                }
                var start = length * startValue;
                var end = length * endValue;
                var newStart = Math.Min(start, end);
                var newEnd = Math.Max(start, end);

                var offset = offsetValue * length;
                newStart += offset;
                newEnd += offset;

                // If the trim path has rotated around the path, we need to shift it back.
                if (newStart >= length && newEnd >= length)
                {
                    newStart = MiscUtils.FloorMod(newStart, length);
                    newEnd = MiscUtils.FloorMod(newEnd, length);
                }

                if (newStart < 0)
                {
                    newStart = MiscUtils.FloorMod(newStart, length);
                }
                if (newEnd < 0)
                {
                    newEnd = MiscUtils.FloorMod(newEnd, length);
                }

                // If the start and end are equals, return an empty path.
                if (newStart == newEnd)
                {
                    path.Reset();
                    LottieLog.EndSection("applyTrimPathIfNeeded");
                    return;
                }

                if (newStart >= newEnd)
                {
                    newStart -= length;
                }

                tempPath = new Path();
                pathMeasure.GetSegment(newStart, newEnd, ref tempPath, true);

                if (newEnd > length)
                {
                    var tempPath2 = new Path();
                    tempPath2.Reset();
                    pathMeasure.GetSegment(0, newEnd % length, ref tempPath2, true);
                    tempPath.AddPath(tempPath2);
                }
                else if (newStart < 0)
                {
                    var tempPath2 = new Path();
                    tempPath2.Reset();
                    pathMeasure.GetSegment(length + newStart, length, ref tempPath2, true);
                    tempPath.AddPath(tempPath2);
                }
            }
            path.Set(tempPath);
            LottieLog.EndSection("applyTrimPathIfNeeded");
        }

        public static Color GetSolidColorBrush(string hex)
        {
            var index = 1; // Skip '#'
            // '#AARRGGBB'
            byte a = 255;
            if (hex.Length == 9)
            {
                a = (byte)Convert.ToUInt32(hex.Substring(index, 2), 16);
                index += 2;
            }
            var r = (byte)Convert.ToUInt32(hex.Substring(index, 2), 16);
            index += 2;
            var g = (byte)Convert.ToUInt32(hex.Substring(index, 2), 16);
            index += 2;
            var b = (byte)Convert.ToUInt32(hex.Substring(index, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }

        public static bool IsAtLeastVersion(int major, int minor, int patch, int minMajor, int minMinor, int minPatch)
        {
            if (major < minMajor)
            {
                return false;
            }
            if (major > minMajor)
            {
                return true;
            }

            if (minor < minMinor)
            {
                return false;
            }
            if (minor > minMinor)
            {
                return true;
            }

            return patch >= minPatch;
        }

        internal static int HashFor(float a, float b, float c, float d)
        {
            int result = 17;
            if (a != 0)
            {
                result = (int)(31 * result * a);
            }
            if (b != 0)
            {
                result = (int)(31 * result * b);
            }
            if (c != 0)
            {
                result = (int)(31 * result * c);
            }
            if (d != 0)
            {
                result = (int)(31 * result * d);
            }
            return result;
        }

        public static float DpScale()
        {
            if (_dpScale == -1)
            {
                _dpScale = (int)DisplayInformation.GetForCurrentView().ResolutionScale / 100f;
            }
            return _dpScale;
        }

        public static float Dpi()
        {
            if (_dpi == -1)
            {
                _dpi = (int)DisplayInformation.GetForCurrentView().LogicalDpi;
            }
            return _dpi;
        }
    }
}