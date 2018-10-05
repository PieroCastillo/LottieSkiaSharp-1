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
using LottieUWP.Model;
using System;

namespace LottieUWP.Parser
{
    static class FontParser
    {
        internal static Font Parse(JsonReader reader)
        {
            string family = null;
            string name = null;
            string style = null;
            float ascent = 0;

            reader.BeginObject();
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "fFamily":
                        family = reader.NextString();
                        break;
                    case "fName":
                        name = reader.NextString();
                        break;
                    case "fStyle":
                        style = reader.NextString();
                        break;
                    case "ascent":
                        ascent = reader.NextDouble();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();

            Enum.TryParse<SkiaSharp.SKTypefaceStyle>(style, true, out var result);

            return new Font(family, name, result, ascent);
        }
    }
}
