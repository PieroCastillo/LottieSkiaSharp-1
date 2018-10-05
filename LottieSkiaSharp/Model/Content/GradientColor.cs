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
using SkiaSharp;
using LottieUWP.Utils;

namespace LottieUWP.Model.Content
{
    public class GradientColor
    {
        private readonly float[] _positions;
        private readonly SKColor[] _colors;

        internal GradientColor(float[] positions, SKColor[] colors)
        {
            _positions = positions;
            _colors = colors;
        }

        internal float[] Positions => _positions;

        internal SKColor[] Colors => _colors;

        internal int Size => _colors.Length;

        internal void Lerp(GradientColor gc1, GradientColor gc2, float progress)
        {
            if (gc1._colors.Length != gc2._colors.Length)
            {
                throw new System.ArgumentException("Cannot interpolate between gradients. Lengths vary (" + gc1._colors.Length + " vs " + gc2._colors.Length + ")");
            }

            for (var i = 0; i < gc1._colors.Length; i++)
            {
                _positions[i] = MiscUtils.Lerp(gc1._positions[i], gc2._positions[i], progress);

                var gamma = GammaEvaluator.Evaluate(progress, gc1._colors[i], gc2._colors[i]);
                
                _colors[i] = gamma;
            }
        }
    }
}