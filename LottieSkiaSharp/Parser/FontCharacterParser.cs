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
using System;
using System.Collections.Generic;
using LottieUWP.Model;
using LottieUWP.Model.Content;

namespace LottieUWP.Parser
{
    static class FontCharacterParser
    {
        internal static FontCharacter Parse(JsonReader reader, LottieComposition composition)
        {
            char character = '\0';
            double size = 0;
            double width = 0;
            String style = null;
            String fontFamily = null;
            List<ShapeGroup> shapes = new List<ShapeGroup>();

            reader.BeginObject();
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "ch":
                        character = reader.NextString()[0];
                        break;
                    case "size":
                        size = reader.NextDouble();
                        break;
                    case "w":
                        width = reader.NextDouble();
                        break;
                    case "style":
                        style = reader.NextString();
                        break;
                    case "fFamily":
                        fontFamily = reader.NextString();
                        break;
                    case "data":
                        reader.BeginObject();
                        while (reader.HasNext())
                        {
                            if ("shapes".Equals(reader.NextName()))
                            {
                                reader.BeginArray();
                                while (reader.HasNext())
                                {
                                    shapes.Add((ShapeGroup)ContentModelParser.Parse(reader, composition));
                                }
                                reader.EndArray();
                            }
                            else
                            {
                                reader.SkipValue();
                            }
                        }
                        reader.EndObject();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();

            Enum.TryParse<SkiaSharp.SKTypefaceStyle>(style, true, out var result);


            return new FontCharacter(shapes, character, size, width,result , fontFamily);
        }
    }
}
