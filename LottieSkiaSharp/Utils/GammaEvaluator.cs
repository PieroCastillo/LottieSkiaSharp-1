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
using SkiaSharp;

namespace LottieUWP.Utils
{
    /// <summary>
    /// Use this instead of ArgbEvaluator because it interpolates through the gamma color
    /// space which looks better to us humans.
    /// <para>
    /// Writted by Romain Guy and Francois Blavoet.
    /// https://androidstudygroup.slack.com/archives/animation/p1476461064000335
    /// </para>
    /// </summary>
    internal static class GammaEvaluator
    {
        // Opto-electronic conversion function for the sRGB color space
        // Takes a gamma-encoded sRGB value and converts it to a linear sRGB value
        private static float OECF_sRGB(float linear)
        {
            // IEC 61966-2-1:1999
            return linear <= 0.0031308f ? linear * 12.92f : (float)(Math.Pow(linear, 1.0f / 2.4f) * 1.055f - 0.055f);
        }

        // Electro-optical conversion function for the sRGB color space
        // Takes a linear sRGB value and converts it to a gamma-encoded sRGB value
        private static float EOCF_sRGB(float srgb)
        {
            // IEC 61966-2-1:1999
            return srgb <= 0.04045f ? srgb / 12.92f : (float)Math.Pow((srgb + 0.055f) / 1.055f, 2.4f);
        }

        internal static SKColor Evaluate(float fraction, SKColor startColor, SKColor endColor)
        {
            return Evaluate(fraction,
                startColor.Alpha / 255.0f, startColor.Red / 255.0f, startColor.Green / 255.0f, startColor.Blue / 255.0f,
                endColor.Alpha / 255.0f, endColor.Red / 255.0f, endColor.Green / 255.0f, endColor.Blue / 255.0f);
        }

        //internal static int evaluate(float fraction, int startInt, int endInt)
        //{
        //    float startA = ((startInt >> 24) & 0xff) / 255.0f;
        //    float startR = ((startInt >> 16) & 0xff) / 255.0f;
        //    float startG = ((startInt >> 8) & 0xff) / 255.0f;
        //    float startB = (startInt & 0xff) / 255.0f;

        //    float endA = ((endInt >> 24) & 0xff) / 255.0f;
        //    float endR = ((endInt >> 16) & 0xff) / 255.0f;
        //    float endG = ((endInt >> 8) & 0xff) / 255.0f;
        //    float endB = (endInt & 0xff) / 255.0f;

        //    return evaluate(fraction, startA, startR, startG, startB, endA, endR, endG, endB);
        //}

        static SKColor Evaluate(float fraction,
                float startA, float startR, float startG, float startB,
                float endA, float endR, float endG, float endB)
        {
            // convert from sRGB to linear
            startR = EOCF_sRGB(startR);
            startG = EOCF_sRGB(startG);
            startB = EOCF_sRGB(startB);

            endR = EOCF_sRGB(endR);
            endG = EOCF_sRGB(endG);
            endB = EOCF_sRGB(endB);

            // compute the interpolated color in linear space
            var a = startA + fraction * (endA - startA);
            var r = startR + fraction * (endR - startR);
            var g = startG + fraction * (endG - startG);
            var b = startB + fraction * (endB - startB);

            // convert back to sRGB in the [0..255] range
            a = a * 255.0f;
            r = OECF_sRGB(r) * 255.0f;
            g = OECF_sRGB(g) * 255.0f;
            b = OECF_sRGB(b) * 255.0f;

            return new SKColor((byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b), (byte)Math.Round(a));
        }
    }
}