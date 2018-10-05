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
namespace LottieUWP.Value
{
    /// <summary>
    /// Delegate interface for <see cref="LottieValueCallback{T}"/>. This is helpful for the Kotlin API because you can use a SAM conversion to write the 
    /// callback as a single abstract method block like this: 
    /// animationView.AddValueCallback(keyPath, LottieProperty.TransformOpacity) { 50 }
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameInfo"></param>
    /// <returns></returns>
    public delegate T SimpleLottieValueCallback<T>(LottieFrameInfo<T> frameInfo);
}
