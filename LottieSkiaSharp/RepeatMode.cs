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
namespace LottieUWP
{
    public enum RepeatMode
    {
        /// <summary>
        /// When the animation reaches the end and <see cref="LottieDrawable.RepeatCount"/> is INFINITE 
        /// or a positive value, the animation restarts from the beginning. 
        /// </summary>
        Restart = 1,
        /// <summary>
        /// When the animation reaches the end and <see cref="LottieDrawable.RepeatCount"/> is INFINITE 
        /// or a positive value, the animation reverses direction on every iteration. 
        /// </summary>
        Reverse = 2
    }
}
