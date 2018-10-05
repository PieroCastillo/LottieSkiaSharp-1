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
namespace LottieUWP.Utils
{
    /// <summary>
    /// Class to calculate the average in a stream of numbers on a continuous basis.
    /// </summary>
    public class MeanCalculator
    {
        private float _sum;
        private int _n;

        public void Add(float number)
        {
            _sum += number;
            _n++;
            if (_n == int.MaxValue)
            {
                _sum /= 2f;
                _n /= 2;
            }
        }

        public float Mean
        {
            get
            {
                if (_n == 0)
                {
                    return 0;
                }
                return _sum / _n;
            }
        }
    }
}
