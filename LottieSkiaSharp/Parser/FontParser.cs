﻿using LottieUWP.Model;
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
