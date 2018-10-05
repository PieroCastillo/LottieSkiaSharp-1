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
    /// Data class describing an image asset exported by bodymovin.
    /// </summary>
    public class LottieImageAsset
    {
        internal LottieImageAsset(int width, int height, string id, string fileName, string dirName)
        {
            Width = width;
            Height = height;
            Id = id;
            FileName = fileName;
            DirName = dirName;
        }

        public int Width { get; }

        public int Height { get; }

        public string Id { get; }

        public string FileName { get; }

        public string DirName { get; }

        /** Pre-set a bitmap for this asset */
        // Returns the bitmap that has been stored for this image asset if one was explicitly set.
        public SKBitmap Bitmap { get; set; }
    }
}