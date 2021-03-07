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
namespace LottieUWP
{
    /// <summary>
    /// Delegate to handle the loading of fonts that are not packaged in the assets of your app or don't
    /// have the same file name.
    /// </summary>
    /// <seealso cref="FontAssetDelegate"></seealso>
    public class FontAssetDelegate
    {
        /// <summary>
        /// Override this if you want to return a Typeface from a font family.
        /// </summary>
        public SKTypeface FetchFont(string fontFamily)
        {
            return null;
        }

        /// <summary>
        /// Override this if you want to specify the asset path for a given font family.
        /// </summary>
        public string GetFontPath(string fontFamily)
        {
            return null;
        }
    }
}
