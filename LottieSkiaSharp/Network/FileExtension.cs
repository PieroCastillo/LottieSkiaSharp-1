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
using System.Diagnostics;

namespace LottieUWP.Network
{
    /// <summary>
    /// Helpers for known Lottie file types.
    /// </summary>
    public class FileExtension
    {
        public static FileExtension Json = new FileExtension(".json");
        public static FileExtension Zip = new FileExtension(".zip");

        public string Extension { get; }

        private FileExtension(string extension)
        {
            Extension = extension;
        }

        public string TempExtension => ".temp" + Extension;

        public override string ToString()
        {
            return Extension;
        }

        public static FileExtension ForFile(string filename)
        {
            foreach (FileExtension e in new[] { Json, Zip })
            {
                if (filename.EndsWith(e.Extension))
                {
                    return e;
                }
            }
            // Default to Json.
            Debug.WriteLine("Unable to find correct extension for " + filename, LottieLog.Tag);
            return Json;
        }
    }
}
