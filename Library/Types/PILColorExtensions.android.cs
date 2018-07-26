using AColor = Android.Graphics.Color;

namespace PILSharp
{
    public static partial class PILColorExtensions
    {
        internal static AColor ToAndroid(this PILColor color) =>
            new AColor
            (
                (byte)(byte.MaxValue * color.R),
                (byte)(byte.MaxValue * color.G),
                (byte)(byte.MaxValue * color.B),
                (byte)(byte.MaxValue * color.A)
            );
    }
}