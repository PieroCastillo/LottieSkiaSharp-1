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

namespace LottieUWP.Animation.Content
{
    internal interface IDrawingContent : IContent
    {
        void Draw(SKCanvas canvas, Matrix3X3 parentMatrix, byte alpha);
        void GetBounds(out SKRect outBounds, Matrix3X3 parentMatrix);
    }
}