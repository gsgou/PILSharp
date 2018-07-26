using CoreGraphics;
using UIKit;

namespace PILSharp
{
    public static partial class PILColorExtensions
    {
        internal static CGColor ToCGColor(this PILColor color) => color.ToUIColor().CGColor;

        internal static UIColor ToUIColor(this PILColor color)  =>
            new UIColor
            (
                (float)color.R,
                (float)color.G,
                (float)color.B,
                (float)color.A
            );
    }
}