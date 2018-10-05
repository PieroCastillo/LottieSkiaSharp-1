namespace LottieUWP.Model
{
    public class Font
    {
        public Font(string family, string name, SkiaSharp.SKTypefaceStyle style, float ascent)
        {
            Family = family;
            Name = name;
            Style = style;
            Ascent = ascent;
        }

        public string Family { get; }

        public string Name { get; }

        public SkiaSharp.SKTypefaceStyle Style { get; }

        internal readonly float Ascent;
    }
}
