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
using System.Linq;
using System.Numerics;
using SkiaSharp;

namespace LottieUWP
{
    public class PathInterpolator : IInterpolator
    {
        public class FullPathIterator : PathIterator
        {
            private readonly Path _path;

            private int _index;

            public FullPathIterator(Path path)
            {
                _path = path;
            }

            public override bool Next()
            {
                _index++;
                if (_index > _path.Contours.Count)
                    return false;
                return true;
            }

            public override bool Done => _index >= _path.Contours.Count;
            public override ContourType CurrentSegment(float[] points)
            {
                var contour = _path.Contours[_index];

                for (var i = 0; i < contour.Points.Length; i++)
                {
                    points[i] = contour.Points[i];
                }
                return contour.Type;
            }
        }
        public class Path
        {
            public interface IContour
            {
                void Transform(Matrix3X3 matrix);
                IContour Copy();
                float[] Points { get; }
                PathIterator.ContourType Type { get; }
                void AddPathSegment(SKPath canvasPathBuilder, ref bool closed);
                void Offset(float dx, float dy);
            }

            class ArcContour : IContour
            {
                private Vector2 _startPoint;
                private Vector2 _endPoint;
                private SKRect _rect;
                private readonly float _startAngle;
                private readonly float _sweepAngle;
                private float _a;
                private float _b;

                public ArcContour(Vector2 startPoint, SKRect rect, float startAngle, float sweepAngle)
                {
                    _startPoint = startPoint;
                    _rect = rect;
                    _a = (float)(rect.Width / 2);
                    _b = (float)(rect.Height / 2);
                    _startAngle = startAngle;
                    _sweepAngle = sweepAngle;

                    _endPoint = GetPointAtAngle(startAngle + sweepAngle);
                }

                public void Transform(Matrix3X3 matrix)
                {
                    _startPoint = matrix.Transform(_startPoint);
                    _endPoint = matrix.Transform(_endPoint);

                    var p1 = new Vector2((float)_rect.Left, (float)_rect.Top);
                    var p2 = new Vector2((float)_rect.Right, (float)_rect.Top);
                    var p3 = new Vector2((float)_rect.Left, (float)_rect.Bottom);
                    var p4 = new Vector2((float)_rect.Right, (float)_rect.Bottom);

                    p1 = matrix.Transform(p1);
                    p2 = matrix.Transform(p2);
                    p3 = matrix.Transform(p3);
                    p4 = matrix.Transform(p4);

                    _a = (p2 - p1).Length() / 2;
                    _b = (p4 - p3).Length() / 2;
                }

                public IContour Copy()
                {
                    return new ArcContour(_startPoint, _rect, _startAngle, _sweepAngle);
                }

                public float[] Points => new[] { _startPoint.X, _startPoint.Y, _endPoint.X, _endPoint.Y };

                public PathIterator.ContourType Type => PathIterator.ContourType.Arc;

                public void AddPathSegment(SKPath canvasPathBuilder, ref bool closed)
                {
                    throw new NotImplementedException();
                }

                public void Offset(float dx, float dy)
                {
                    _startPoint.X += dx;
                    _startPoint.Y += dy;
                    _endPoint.X += dx;
                    _endPoint.Y += dy;
                }

                private Vector2 GetPointAtAngle(float t)
                {
                    var u = Math.Tan(MathExt.ToRadians(t) / 2);

                    var u2 = u * u;

                    var x = _a * (1 - u2) / (u2 + 1);
                    var y = 2 * _b * u / (u2 + 1);

                    return new Vector2((float)(_rect.Left + _a + x), (float)(_rect.Top + _b + y));
                }
            }

            internal class BezierContour : IContour
            {
                private Vector2 _control1;
                private Vector2 _control2;
                private Vector2 _vertex;

                public BezierContour(Vector2 control1, Vector2 control2, Vector2 vertex)
                {
                    _control1 = control1;
                    _control2 = control2;
                    _vertex = vertex;
                }

                public void Transform(Matrix3X3 matrix)
                {
                    _control1 = matrix.Transform(_control1);
                    _control2 = matrix.Transform(_control2);
                    _vertex = matrix.Transform(_vertex);
                }

                public IContour Copy()
                {
                    return new BezierContour(_control1, _control2, _vertex);
                }

                internal static double BezLength(float c0X, float c0Y, float c1X, float c1Y, float c2X, float c2Y, float c3X, float c3Y)
                {
                    const double steps = 1000d; // TODO: improve

                    var length = 0d;
                    float prevPtX = 0;
                    float prevPtY = 0;

                    for (var i = 0d; i < steps; i++)
                    {
                        var pt = GetPointAtT(c0X, c0Y, c1X, c1Y, c2X, c2Y, c3X, c3Y, i / steps);

                        if (i > 0)
                        {
                            var x = pt.X - prevPtX;
                            var y = pt.Y - prevPtY;
                            length = length + Math.Sqrt(x * x + y * y);
                        }

                        prevPtX = pt.X;
                        prevPtY = pt.Y;
                    }
                    return length;
                }

                private static Vector2 GetPointAtT(float c0X, float c0Y, float c1X, float c1Y, float c2X, float c2Y, float c3X, float c3Y, double t)
                {
                    var t1 = 1d - t;

                    if (t1 < 5e-6)
                    {
                        t = 1.0;
                        t1 = 0.0;
                    }

                    var t13 = t1 * t1 * t1;
                    var t13A = 3 * t * (t1 * t1);
                    var t13B = 3 * t * t * t1;
                    var t13C = t * t * t;

                    var ptX = (float)(c0X * t13 + t13A * c1X + t13B * c2X + t13C * c3X);
                    var ptY = (float)(c0Y * t13 + t13A * c1Y + t13B * c2Y + t13C * c3Y);

                    return new Vector2(ptX, ptY);
                }

                public float[] Points => new[] { _control1.X, _control1.Y, _control2.X, _control2.Y, _vertex.X, _vertex.Y };

                public PathIterator.ContourType Type => PathIterator.ContourType.Bezier;

                public void AddPathSegment(SKPath canvasPathBuilder, ref bool closed)
                {
                    throw new NotImplementedException();

                    closed = false;
                }

                public void Offset(float dx, float dy)
                {
                    _control1.X += dx;
                    _control1.Y += dy;
                    _control2.X += dx;
                    _control2.Y += dy;
                    _vertex.X += dx;
                    _vertex.Y += dy;
                }
            }

            class LineContour : IContour
            {
                private readonly float[] _points = new float[2];

                public LineContour(float x, float y)
                {
                    _points[0] = x;
                    _points[1] = y;
                }

                public void Transform(Matrix3X3 matrix)
                {
                    var p = new Vector2(_points[0], _points[1]);

                    p = matrix.Transform(p);

                    _points[0] = p.X;
                    _points[1] = p.Y;
                }

                public IContour Copy()
                {
                    return new LineContour(_points[0], _points[1]);
                }

                public float[] Points => _points;

                public PathIterator.ContourType Type => PathIterator.ContourType.Line;

                public void AddPathSegment(SKPath canvasPathBuilder, ref bool closed)
                {
                    throw new NotImplementedException();

                    closed = false;
                }

                public void Offset(float dx, float dy)
                {
                    _points[0] += dx;
                    _points[1] += dy;
                }
            }

            class MoveToContour : IContour
            {
                private readonly float[] _points = new float[2];

                public MoveToContour(float x, float y)
                {
                    _points[0] = x;
                    _points[1] = y;
                }

                public float[] Points => _points;

                public PathIterator.ContourType Type => PathIterator.ContourType.MoveTo;

                public IContour Copy()
                {
                    return new MoveToContour(_points[0], _points[1]);
                }

                public void AddPathSegment(SKPath canvasPathBuilder, ref bool closed)
                {
                    if (!closed)
                    {
                        //canvasPathBuilder.Close();
                    }
                    else
                    {
                        closed = false;
                    }
                    throw new NotImplementedException();
                }

                public void Offset(float dx, float dy)
                {
                    _points[0] += dx;
                    _points[1] += dy;
                }

                public void Transform(Matrix3X3 matrix)
                {
                    var p = new Vector2(_points[0], _points[1]);

                    p = matrix.Transform(p);

                    _points[0] = p.X;
                    _points[1] = p.Y;
                }
            }

            class CloseContour : IContour
            {
                public float[] Points => new float[0];

                public PathIterator.ContourType Type => PathIterator.ContourType.Close;

                public IContour Copy()
                {
                    return new CloseContour();
                }

                public void AddPathSegment(SKPath canvasPathBuilder, ref bool closed)
                {
                    if (!closed)
                    {
                        throw new NotImplementedException();
                        closed = true;
                    }
                }

                public void Offset(float dx, float dy)
                {
                }

                public void Transform(Matrix3X3 matrix)
                {
                }
            }

            class OpContour : IContour
            {
                Path path1;
                Path path2;

                public OpContour(Path path1, Path path2, SKPathOp opType)
                {
                    this.path1 = path1;
                    this.path2 = path2;
                    OpType = opType;
                }

                public float[] Points => Array.Empty<float>();
                public SKPathOp OpType { get; }
                public PathIterator.ContourType Type => PathIterator.ContourType.Op;

                public void AddPathSegment(SKPath canvasPathBuilder, ref bool closed)
                {
                     throw new NotImplementedException();
                }

                public IContour Copy()
                {
                    return new OpContour(path1, path2, OpType);
                }

                public void Offset(float dx, float dy)
                {
                    path1.Offset(dx, dy);
                    path2.Offset(dx, dy);
                }

                public void Transform(Matrix3X3 matrix)
                {
                    path1.Transform(matrix);
                    path2.Transform(matrix);
                }
            }

            public List<IContour> Contours { get; }

            public Path()
            {
                Contours = new List<IContour>();
            }

            public void Set(Path path)
            {
                Contours.Clear();
                Contours.AddRange(path.Contours.Select(p => p.Copy()));
            }

            public void Transform(Matrix3X3 matrix)
            {
                for (var j = 0; j < Contours.Count; j++)
                {
                    Contours[j].Transform(matrix);
                }
            }


            public void ComputeBounds(out SKRect rect)
            {
                rect = SKRect.Empty;
                if (Contours.Count == 0)
                {
                    RectExt.Set(ref rect, 0, 0, 0, 0);
                    return;
                }


            }

            public void AddPath(Path path, Matrix3X3 matrix)
            {
                var pathCopy = new Path();
                pathCopy.Set(path);
                pathCopy.Transform(matrix);
                Contours.AddRange(pathCopy.Contours);
            }

            public void AddPath(Path path)
            {
                Contours.AddRange(path.Contours.Select(p => p.Copy()).ToList());
            }

            public void Reset()
            {
                Contours.Clear();
            }

            public void MoveTo(float x, float y)
            {
                Contours.Add(new MoveToContour(x, y));
            }

            public void CubicTo(float x1, float y1, float x2, float y2, float x3, float y3)
            {
                var bezier = new BezierContour(
                    new Vector2(x1, y1),
                    new Vector2(x2, y2),
                    new Vector2(x3, y3)
                );
                Contours.Add(bezier);
            }

            public void LineTo(float x, float y)
            {
                var newLine = new LineContour(x, y);
                Contours.Add(newLine);
            }

            public void Offset(float dx, float dy)
            {
                for (var i = 0; i < Contours.Count; i++)
                {
                    Contours[i].Offset(dx, dy);
                }
            }

            public void Close()
            {
                Contours.Add(new CloseContour());
            }

            /*
             Set this path to the result of applying the Op to the two specified paths. The resulting path will be constructed from non-overlapping contours. The curve order is reduced where possible so that cubics may be turned into quadratics, and quadratics maybe turned into lines.
              Path1: The first operand (for difference, the minuend)
              Path2: The second operand (for difference, the subtrahend)
            */
            public void Op(Path path1, Path path2, SKPathOp op)
            {
                Contours.Add(new OpContour(path1, path2, op));
            }

            public void ArcTo(float x, float y, SKRect rect, float startAngle, float sweepAngle)
            {
                var newArc = new ArcContour(new Vector2(x, y), rect, startAngle, sweepAngle);
                Contours.Add(newArc);
            }

            public float[] Approximate(float precision)
            {
                var pathIteratorFactory = new CachedPathIteratorFactory(new FullPathIterator(this));
                var pathIterator = pathIteratorFactory.Iterator();
                float[] points = new float[8];
                var segmentPoints = new List<Vector2>();
                var lengths = new List<float>();
                float errorSquared = precision * precision;
                while (!pathIterator.Done)
                {
                    var type = pathIterator.CurrentSegment(points);
                    switch (type)
                    {
                        case PathIterator.ContourType.MoveTo:
                            AddMove(segmentPoints, lengths, points);
                            break;
                        case PathIterator.ContourType.Close:
                            AddLine(segmentPoints, lengths, points);
                            break;
                        case PathIterator.ContourType.Line:
                            AddLine(segmentPoints, lengths, points.Skip(2).ToArray());
                            break;
                        case PathIterator.ContourType.Arc:
                            AddBezier(points, QuadraticBezierCalculation, segmentPoints, lengths, errorSquared, false);
                            break;
                        case PathIterator.ContourType.Bezier:
                            AddBezier(points, CubicBezierCalculation, segmentPoints, lengths, errorSquared, true);
                            break;
                    }
                    pathIterator.Next();
                }

                if (!segmentPoints.Any())
                {
                    int numVerbs = Contours.Count;
                    if (numVerbs == 1)
                    {
                        AddMove(segmentPoints, lengths, Contours[0].Points);
                    }
                    else
                    {
                        // Invalid or empty path. Fall back to point(0,0)
                        AddMove(segmentPoints, lengths, new[] { 0.0f, 0.0f });
                    }
                }

                float totalLength = lengths.Last();
                if (totalLength == 0)
                {
                    // Lone Move instructions should still be able to animate at the same value.
                    segmentPoints.Add(segmentPoints.Last());
                    lengths.Add(1);
                    totalLength = 1;
                }

                var numPoints = segmentPoints.Count;
                var approximationArraySize = numPoints * 3;

                var approximation = new float[approximationArraySize];

                int approximationIndex = 0;
                for (var i = 0; i < numPoints; i++)
                {
                    var point = segmentPoints[i];
                    approximation[approximationIndex++] = lengths[i] / totalLength;
                    approximation[approximationIndex++] = point.X;
                    approximation[approximationIndex++] = point.Y;
                }

                return approximation;
            }

            static float QuadraticCoordinateCalculation(float t, float p0, float p1, float p2)
            {
                float oneMinusT = 1 - t;
                return oneMinusT * ((oneMinusT * p0) + (t * p1)) + t * ((oneMinusT * p1) + (t * p2));
            }

            static Vector2 QuadraticBezierCalculation(float t, float[] points)
            {
                float x = QuadraticCoordinateCalculation(t, points[0], points[2], points[4]);
                float y = QuadraticCoordinateCalculation(t, points[1], points[3], points[5]);
                return new Vector2(x, y);
            }

            static float CubicCoordinateCalculation(float t, float p0, float p1, float p2, float p3)
            {
                float oneMinusT = 1 - t;
                float oneMinusTSquared = oneMinusT * oneMinusT;
                float oneMinusTCubed = oneMinusTSquared * oneMinusT;
                float tSquared = t * t;
                float tCubed = tSquared * t;
                return (oneMinusTCubed * p0) + (3 * oneMinusTSquared * t * p1)
                                             + (3 * oneMinusT * tSquared * p2) + (tCubed * p3);
            }

            static Vector2 CubicBezierCalculation(float t, float[] points)
            {
                float x = CubicCoordinateCalculation(t, points[0], points[2], points[4], points[6]);
                float y = CubicCoordinateCalculation(t, points[1], points[3], points[5], points[7]);
                return new Vector2(x, y);
            }

            static void AddMove(List<Vector2> segmentPoints, List<float> lengths, float[] point)
            {
                float length = 0;
                if (lengths.Any())
                {
                    length = lengths.Last();
                }
                segmentPoints.Add(new Vector2(point[0], point[1]));
                lengths.Add(length);
            }

            static void AddLine(List<Vector2> segmentPoints, List<float> lengths, float[] toPoint)
            {
                if (!segmentPoints.Any())
                {
                    segmentPoints.Add(Vector2.Zero);
                    lengths.Add(0);
                }
                else if (segmentPoints.Last().X == toPoint[0] && segmentPoints.Last().Y == toPoint[1])
                {
                    return; // Empty line
                }

                var vector2 = new Vector2(toPoint[0], toPoint[1]);
                float length = lengths.Last() + (vector2 - segmentPoints.Last()).Length();
                segmentPoints.Add(vector2);
                lengths.Add(length);
            }

            delegate Vector2 BezierCalculation(float t, float[] points);

            static void AddBezier(float[] points, BezierCalculation bezierFunction, List<Vector2> segmentPoints, List<float> lengths, float errorSquared, bool doubleCheckDivision)
            {
                points[7] = points[5];
                points[6] = points[4];
                points[5] = points[3];
                points[4] = points[2];
                points[3] = points[1];
                points[2] = points[0];
                points[1] = 0;
                points[0] = 0;

                var tToPoint = new List<KeyValuePair<float, Vector2>>
            {
                new KeyValuePair<float, Vector2>(0, bezierFunction(0, points)),
                new KeyValuePair<float, Vector2>(1, bezierFunction(1, points))
            };

                for (int i = 0; i < tToPoint.Count - 1; i++)
                {
                    bool needsSubdivision;
                    do
                    {
                        needsSubdivision = SubdividePoints(points, bezierFunction, tToPoint[i].Key, tToPoint[i].Value, tToPoint[i + 1].Key,
                            tToPoint[i + 1].Value, out var midT, out var midPoint, errorSquared);
                        if (!needsSubdivision && doubleCheckDivision)
                        {
                            needsSubdivision = SubdividePoints(points, bezierFunction, tToPoint[i].Key, tToPoint[i].Value, midT,
                                midPoint, out _, out _, errorSquared);
                            if (needsSubdivision)
                            {
                                // Found an inflection point. No need to double-check.
                                doubleCheckDivision = false;
                            }
                        }

                        if (needsSubdivision)
                        {
                            tToPoint.Insert(i + 1, new KeyValuePair<float, Vector2>(midT, midPoint));
                        }
                    } while (needsSubdivision);
                }

                // Now that each division can use linear interpolation with less than the allowed error
                foreach (var iter in tToPoint)
                {
                    AddLine(segmentPoints, lengths, new[] { iter.Value.X, iter.Value.Y });
                }
            }

            private static bool SubdividePoints(float[] points, BezierCalculation bezierFunction, float t0, Vector2 p0, float t1, Vector2 p1, out float midT, out Vector2 midPoint, float errorSquared)
            {
                midT = (t1 + t0) / 2;
                float midX = (p1.X + p0.X) / 2;
                float midY = (p1.Y + p0.Y) / 2;

                midPoint = bezierFunction(midT, points);
                float xError = midPoint.X - midX;
                float yError = midPoint.Y - midY;
                float midErrorSquared = (xError * xError) + (yError * yError);
                return midErrorSquared > errorSquared;
            }
        }
        /// <summary>
        /// Class that returns iterators for a given path. These iterators are lightweight and can be reused
        /// multiple times to iterate over the path.
        /// </summary>
        public class CachedPathIteratorFactory
        {
            private readonly PathIterator.ContourType[] _types;
            private readonly float[][] _coordinates;
            private readonly float[] _segmentsLength;

            public CachedPathIteratorFactory(PathIterator iterator)
            {
                var typesArray = new List<PathIterator.ContourType>();
                var pointsArray = new List<float[]>();
                var points = new float[6];
                while (!iterator.Done)
                {
                    var type = iterator.CurrentSegment(points);
                    var nPoints = GetNumberOfPoints(type) * 2; // 2 coordinates per point

                    typesArray.Add(type);
                    var itemPoints = new float[nPoints];
                    Array.Copy(points, 0, itemPoints, 0, nPoints);
                    pointsArray.Add(itemPoints);
                    iterator.Next();
                }

                _types = new PathIterator.ContourType[typesArray.Count];
                _coordinates = new float[_types.Length][];
                for (var i = 0; i < typesArray.Count; i++)
                {
                    _types[i] = typesArray[i];
                    _coordinates[i] = pointsArray[i];
                }

                // Do measurement
                _segmentsLength = new float[_types.Length];

                // Curves that we can reuse to estimate segments length
                float lastX = 0;
                float lastY = 0;
                for (var i = 0; i < _types.Length; i++)
                {
                    switch (_types[i])
                    {
                        case PathIterator.ContourType.Bezier:
                            _segmentsLength[i] = (float)Path.BezierContour.BezLength(lastX, lastY,
                                _coordinates[i][0], _coordinates[i][1],
                                _coordinates[i][2], _coordinates[i][3],
                                lastX = _coordinates[i][4], lastY = _coordinates[i][5]);
                            break;
                        case PathIterator.ContourType.Arc:
                            _segmentsLength[i] = (float)Path.BezierContour.BezLength(lastX, lastY,
                                lastX + 2 * (_coordinates[i][0] - lastX) / 3, lastY + 2 * (_coordinates[i][1] - lastY) / 3,
                                _coordinates[i][2] + 2 * (_coordinates[i][0] - _coordinates[i][2]) / 3, _coordinates[i][3] + 2 * (_coordinates[i][1] - _coordinates[i][3]) / 3,
                                lastX = _coordinates[i][2], lastY = _coordinates[i][3]);
                            break;
                        case PathIterator.ContourType.Close:
                            _segmentsLength[i] = Vector2.Distance(new Vector2(lastX, lastY),
                                new Vector2(lastX = _coordinates[0][0], lastY = _coordinates[0][1]));
                            _coordinates[i] = new float[2];
                            // We convert a CloseContour segment to a LineContour so we do not have to worry
                            // about this special case in the rest of the code.
                            _types[i] = PathIterator.ContourType.Line;
                            _coordinates[i][0] = _coordinates[0][0];
                            _coordinates[i][1] = _coordinates[0][1];
                            break;
                        case PathIterator.ContourType.MoveTo:
                            _segmentsLength[i] = 0;
                            lastX = _coordinates[i][0];
                            lastY = _coordinates[i][1];
                            break;
                        case PathIterator.ContourType.Line:
                            _segmentsLength[i] = Vector2.Distance(new Vector2(lastX, lastY), new Vector2(_coordinates[i][0], _coordinates[i][1]));
                            lastX = _coordinates[i][0];
                            lastY = _coordinates[i][1];
                            break;
                    }
                }
            }

            private static void QuadCurveSegment(float[] coords, float t0, float t1)
            {
                // Calculate X and Y at 0.5 (We'll use this to reconstruct the control point later)
                var mt = t0 + (t1 - t0) / 2;
                var mu = 1 - mt;
                var mx = mu * mu * coords[0] + 2 * mu * mt * coords[2] + mt * mt * coords[4];
                var my = mu * mu * coords[1] + 2 * mu * mt * coords[3] + mt * mt * coords[5];

                var u0 = 1 - t0;
                var u1 = 1 - t1;

                // coords at t0
                coords[0] = coords[0] * u0 * u0 + coords[2] * 2 * t0 * u0 + coords[4] * t0 * t0;
                coords[1] = coords[1] * u0 * u0 + coords[3] * 2 * t0 * u0 + coords[5] * t0 * t0;

                // coords at t1
                coords[4] = coords[0] * u1 * u1 + coords[2] * 2 * t1 * u1 + coords[4] * t1 * t1;
                coords[5] = coords[1] * u1 * u1 + coords[3] * 2 * t1 * u1 + coords[5] * t1 * t1;

                // estimated control point at t'=0.5
                coords[2] = 2 * mx - coords[0] / 2 - coords[4] / 2;
                coords[3] = 2 * my - coords[1] / 2 - coords[5] / 2;
            }

            private static void CubicCurveSegment(float[] coords, float t0, float t1)
            {
                // http://stackoverflow.com/questions/11703283/cubic-bezier-curve-segment
                var u0 = 1 - t0;
                var u1 = 1 - t1;

                // Calculate the points at t0 and t1 for the quadratic curves formed by (P0, P1, P2) and
                // (P1, P2, P3)
                var qxa = coords[0] * u0 * u0 + coords[2] * 2 * t0 * u0 + coords[4] * t0 * t0;
                var qxb = coords[0] * u1 * u1 + coords[2] * 2 * t1 * u1 + coords[4] * t1 * t1;
                var qxc = coords[2] * u0 * u0 + coords[4] * 2 * t0 * u0 + coords[6] * t0 * t0;
                var qxd = coords[2] * u1 * u1 + coords[4] * 2 * t1 * u1 + coords[6] * t1 * t1;

                var qya = coords[1] * u0 * u0 + coords[3] * 2 * t0 * u0 + coords[5] * t0 * t0;
                var qyb = coords[1] * u1 * u1 + coords[3] * 2 * t1 * u1 + coords[5] * t1 * t1;
                var qyc = coords[3] * u0 * u0 + coords[5] * 2 * t0 * u0 + coords[7] * t0 * t0;
                var qyd = coords[3] * u1 * u1 + coords[5] * 2 * t1 * u1 + coords[7] * t1 * t1;

                // Linear interpolation
                coords[0] = qxa * u0 + qxc * t0;
                coords[1] = qya * u0 + qyc * t0;

                coords[2] = qxa * u1 + qxc * t1;
                coords[3] = qya * u1 + qyc * t1;

                coords[4] = qxb * u0 + qxd * t0;
                coords[5] = qyb * u0 + qyd * t0;

                coords[6] = qxb * u1 + qxd * t1;
                coords[7] = qyb * u1 + qyd * t1;
            }

            /// <summary>
            /// Returns the end point of a given segment
            /// </summary>
            /// <param name="type"> the segment type </param>
            /// <param name="coords"> the segment coordinates array </param>
            /// <param name="point"> the return array where the point will be stored </param>
            private static void GetShapeEndPoint(PathIterator.ContourType type, float[] coords, float[] point)
            {
                // start index of the end point for the segment type
                var pointIndex = (GetNumberOfPoints(type) - 1) * 2;
                point[0] = coords[pointIndex];
                point[1] = coords[pointIndex + 1];
            }

            /// <summary>
            /// Returns the number of points stored in a coordinates array for the given segment type.
            /// </summary>
            private static int GetNumberOfPoints(PathIterator.ContourType segmentType)
            {
                switch (segmentType)
                {
                    case PathIterator.ContourType.Arc:
                        return 2;
                    case PathIterator.ContourType.Bezier:
                        return 3;
                    case PathIterator.ContourType.Close:
                        return 0;
                    default:
                        return 1;
                }
            }

            /// <summary>
            /// Returns the estimated position along a path of the given length.
            /// </summary>
            private void GetPointAtLength(PathIterator.ContourType type, float[] coords, float lastX, float lastY, float t, float[] point)
            {
                if (type == PathIterator.ContourType.Line)
                {
                    point[0] = lastX + (coords[0] - lastX) * t;
                    point[1] = lastY + (coords[1] - lastY) * t;
                    // Return here, since we do not need a shape to estimate
                    return;
                }

                var curve = new float[8];
                var lastPointIndex = (GetNumberOfPoints(type) - 1) * 2;

                Array.Copy(coords, 0, curve, 2, coords.Length);
                curve[0] = lastX;
                curve[1] = lastY;
                if (type == PathIterator.ContourType.Bezier)
                {
                    CubicCurveSegment(curve, 0f, t);
                }
                else
                {
                    QuadCurveSegment(curve, 0f, t);
                }

                point[0] = curve[2 + lastPointIndex];
                point[1] = curve[2 + lastPointIndex + 1];
            }

            public CachedPathIterator Iterator()
            {
                return new CachedPathIterator(this);
            }

            /// <summary>
            /// Class that allows us to iterate over a path multiple times
            /// </summary>
            public class CachedPathIterator : PathIterator
            {
                private readonly CachedPathIteratorFactory _outerInstance;

                private int _nextIndex;

                /// <summary>
                /// Current segment type.
                /// </summary>
                /// <seealso cref="PathIterator"/>
                private ContourType _currentType;

                /// <summary>
                /// Stores the coordinates array of the current segment. The number of points stored depends
                /// on the segment type.
                /// </summary>
                /// <seealso cref="PathIterator"></seealso>
                private readonly float[] _currentCoords = new float[6];

                private float _currentSegmentLength;

                /// <summary>
                /// Current segment length offset. When asking for the length of the current segment, the
                /// length will be reduced by this amount. This is useful when we are only using portions of
                /// the segment.
                /// </summary>
                /// <seealso cref="JumpToSegment(float)"></seealso>
                private float _mOffsetLength;

                /// <summary>
                /// Point where the current segment started </summary>
                private readonly float[] _mLastPoint = new float[2];

                private bool _isIteratorDone;

                internal CachedPathIterator(CachedPathIteratorFactory outerInstance)
                {
                    _outerInstance = outerInstance;
                    Next();
                }

                public float CurrentSegmentLength => _currentSegmentLength;

                public override bool Done => _isIteratorDone;

                public override bool Next()
                {
                    if (_nextIndex >= _outerInstance._types.Length)
                    {
                        _isIteratorDone = true;
                        return false;
                    }

                    if (_nextIndex >= 1)
                    {
                        // We've already called next() once so there is a previous segment in this path.
                        // We want to get the coordinates where the path ends.
                        GetShapeEndPoint(_currentType, _currentCoords, _mLastPoint);
                    }
                    else
                    {
                        // This is the first segment, no previous point so initialize to 0, 0
                        _mLastPoint[0] = _mLastPoint[1] = 0f;
                    }
                    _currentType = _outerInstance._types[_nextIndex];
                    _currentSegmentLength = _outerInstance._segmentsLength[_nextIndex] - _mOffsetLength;

                    if (_mOffsetLength > 0f && (_currentType == ContourType.Bezier || _currentType == ContourType.Arc))
                    {
                        // We need to skip part of the start of the current segment (because
                        // mOffsetLength > 0)
                        var points = new float[8];

                        if (_nextIndex < 1)
                        {
                            points[0] = points[1] = 0f;
                        }
                        else
                        {
                            GetShapeEndPoint(_outerInstance._types[_nextIndex - 1],
                                _outerInstance._coordinates[_nextIndex - 1], points);
                        }

                        Array.Copy(_outerInstance._coordinates[_nextIndex], 0, points, 2,
                            _outerInstance._coordinates[_nextIndex].Length);
                        var t0 = (_outerInstance._segmentsLength[_nextIndex] - _currentSegmentLength) /
                                 _outerInstance._segmentsLength[_nextIndex];
                        if (_currentType == ContourType.Bezier)
                        {
                            CubicCurveSegment(points, t0, 1f);
                        }
                        else
                        {
                            QuadCurveSegment(points, t0, 1f);
                        }
                        Array.Copy(points, 2, _currentCoords, 0, _outerInstance._coordinates[_nextIndex].Length);
                    }
                    else
                    {
                        Array.Copy(_outerInstance._coordinates[_nextIndex], 0, _currentCoords, 0,
                            _outerInstance._coordinates[_nextIndex].Length);
                    }

                    _mOffsetLength = 0f;
                    _nextIndex++;
                    return true;
                }

                public override ContourType CurrentSegment(float[] coords)
                {
                    Array.Copy(_currentCoords, 0, coords, 0, GetNumberOfPoints(_currentType) * 2);
                    return _currentType;
                }

                /// <summary>
                /// Returns the point where the current segment ends
                /// </summary>
                public void GetCurrentSegmentEnd(float[] point)
                {
                    point[0] = _mLastPoint[0];
                    point[1] = _mLastPoint[1];
                }

                /// <summary>
                /// Restarts the iterator and jumps all the segments of this path up to the length value.
                /// </summary>
                public void JumpToSegment(float length)
                {
                    _isIteratorDone = false;
                    if (length <= 0f)
                    {
                        _nextIndex = 0;
                        return;
                    }

                    float accLength = 0;
                    var lastPoint = new float[2];
                    for (_nextIndex = 0; _nextIndex < _outerInstance._types.Length; _nextIndex++)
                    {
                        var segmentLength = _outerInstance._segmentsLength[_nextIndex];
                        if (accLength + segmentLength >= length && _outerInstance._types[_nextIndex] != ContourType.MoveTo)
                        {
                            var estimatedPoint = new float[2];
                            _outerInstance.GetPointAtLength(_outerInstance._types[_nextIndex],
                                _outerInstance._coordinates[_nextIndex], lastPoint[0], lastPoint[1],
                                (length - accLength) / segmentLength, estimatedPoint);

                            // This segment makes us go further than length so we go back one step,
                            // set a moveto and offset the length of the next segment by the length
                            // of this segment that we've already used.
                            _currentType = ContourType.MoveTo;
                            _currentCoords[0] = estimatedPoint[0];
                            _currentCoords[1] = estimatedPoint[1];
                            _currentSegmentLength = 0;

                            // We need to offset next path length to account for the segment we've just
                            // skipped.
                            _mOffsetLength = length - accLength;
                            return;
                        }
                        accLength += segmentLength;
                        GetShapeEndPoint(_outerInstance._types[_nextIndex], _outerInstance._coordinates[_nextIndex],
                            lastPoint);
                    }
                }

                /// <summary>
                /// Returns the current segment up to certain length. If the current segment is shorter than
                /// length, then the whole segment is returned. The segment coordinates are copied into the
                /// coords array.
                /// </summary>
                /// <returns> the segment type </returns>
                public ContourType CurrentSegment(float[] coords, float length)
                {
                    var type = CurrentSegment(coords);
                    // If the length is greater than the current segment length, no need to find
                    // the cut point. Same if this is a SEG_MOVETO.
                    if (_currentSegmentLength <= length || type == ContourType.MoveTo)
                    {
                        return type;
                    }

                    var t = length / CurrentSegmentLength;

                    // We find at which offset the end point is located within the coords array and set
                    // a new end point to cut the segment short
                    switch (type)
                    {
                        case ContourType.Bezier:
                        case ContourType.Arc:
                            var curve = new float[8];
                            curve[0] = _mLastPoint[0];
                            curve[1] = _mLastPoint[1];
                            Array.Copy(coords, 0, curve, 2, coords.Length);
                            if (type == ContourType.Bezier)
                            {
                                CubicCurveSegment(curve, 0f, t);
                            }
                            else
                            {
                                QuadCurveSegment(curve, 0f, t);
                            }
                            Array.Copy(curve, 2, coords, 0, coords.Length);
                            break;
                        default:
                            var point = new float[2];
                            _outerInstance.GetPointAtLength(type, coords, _mLastPoint[0], _mLastPoint[1], t, point);
                            coords[0] = point[0];
                            coords[1] = point[1];
                            break;
                    }

                    return type;
                }
            }
        }

        public PathInterpolator(float controlX1, float controlY1, float controlX2, float controlY2)
        {
            InitCubic(controlX1, controlY1, controlX2, controlY2);
        }

        private static readonly float Precision = 0.002f;

        private float[] _mX; // x coordinates in the line

        private float[] _mY; // y coordinates in the line

        private void InitCubic(float x1, float y1, float x2, float y2)
        {
            Path path = new Path();
            path.MoveTo(0, 0);
            path.CubicTo(x1, y1, x2, y2, 1f, 1f);
            InitPath(path);
        }

        private void InitPath(Path path)
        {
            float[] pointComponents = path.Approximate(Precision);

            int numPoints = pointComponents.Length / 3;
            if (pointComponents[1] != 0 || pointComponents[2] != 0
                                        || pointComponents[pointComponents.Length - 2] != 1
                                        || pointComponents[pointComponents.Length - 1] != 1)
            {
                //throw new ArgumentException("The Path must start at (0,0) and end at (1,1)");
            }

            _mX = new float[numPoints];
            _mY = new float[numPoints];
            float prevX = 0;
            float prevFraction = 0;
            int componentIndex = 0;
            for (int i = 0; i < numPoints; i++)
            {
                float fraction = pointComponents[componentIndex++];
                float x = pointComponents[componentIndex++];
                float y = pointComponents[componentIndex++];
                if (fraction == prevFraction && x != prevX)
                {
                    throw new ArgumentException("The Path cannot have discontinuity in the X axis.");
                }
                if (x < prevX)
                {
                    //throw new ArgumentException("The Path cannot loop back on itself.");
                }
                _mX[i] = x;
                _mY[i] = y;
                prevX = x;
                prevFraction = fraction;
            }
        }

        public float GetInterpolation(float t)
        {
            if (t <= 0 || float.IsNaN(t))
            {
                return 0;
            }
            if (t >= 1)
            {
                return 1;
            }
            // Do a binary search for the correct x to interpolate between.
            int startIndex = 0;
            int endIndex = _mX.Length - 1;

            while (endIndex - startIndex > 1)
            {
                int midIndex = (startIndex + endIndex) / 2;
                if (t < _mX[midIndex])
                {
                    endIndex = midIndex;
                }
                else
                {
                    startIndex = midIndex;
                }
            }

            float xRange = _mX[endIndex] - _mX[startIndex];
            if (xRange == 0)
            {
                return _mY[startIndex];
            }

            float tInRange = t - _mX[startIndex];
            float fraction = tInRange / xRange;

            float startY = _mY[startIndex];
            float endY = _mY[endIndex];
            return startY + (fraction * (endY - startY));
        }
    }
}