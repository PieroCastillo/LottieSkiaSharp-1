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
using System.Collections.Generic;
using LottieUWP.Model.Content;

namespace LottieUWP.Parser
{
    static class ShapeGroupParser
    {
        internal static ShapeGroup Parse(JsonReader reader, LottieComposition composition)
        {
            string name = null;
            List<IContentModel> items = new List<IContentModel>();

            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "nm":
                        name = reader.NextString();
                        break;
                    case "it":
                        reader.BeginArray();
                        while (reader.HasNext())
                        {
                            IContentModel newItem = ContentModelParser.Parse(reader, composition);
                            if (newItem != null)
                            {
                                items.Add(newItem);
                            }
                        }
                        reader.EndArray();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            return new ShapeGroup(name, items);
        }
    }
}
