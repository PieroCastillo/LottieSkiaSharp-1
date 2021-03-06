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
using System.Numerics;

namespace LottieUWP.Model
{
    public class CubicCurveData
    {
        private Vector2 _controlPoint1;
        private Vector2 _controlPoint2;
        private Vector2 _vertex;

        internal CubicCurveData()
        {
            _controlPoint1 = new Vector2();
            _controlPoint2 = new Vector2();
            _vertex = new Vector2();
        }

        internal CubicCurveData(Vector2 controlPoint1, Vector2 controlPoint2, Vector2 vertex)
        {
            _controlPoint1 = controlPoint1;
            _controlPoint2 = controlPoint2;
            _vertex = vertex;
        }

        internal void SetControlPoint1(float x, float y)
        {
            _controlPoint1.X = x;
            _controlPoint1.Y = y;
        }

        internal Vector2 ControlPoint1 => _controlPoint1;

        internal void SetControlPoint2(float x, float y)
        {
            _controlPoint2.X = x;
            _controlPoint2.Y = y;
        }

        internal Vector2 ControlPoint2 => _controlPoint2;

        internal void SetVertex(float x, float y)
        {
            _vertex.X = x;
            _vertex.Y = y;
        }

        internal Vector2 Vertex => _vertex;
    }
}