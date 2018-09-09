using System;

namespace PILSharp
{
    public struct PILThickness : IEquatable<PILThickness>
    {
        // Try to match Xamarin.Forms:
        // https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/Thickness.cs

        public double Left { get; set; }

        public double Top { get; set; }

        public double Right { get; set; }

        public double Bottom { get; set; }

        public double HorizontalThickness =>
            Left + Right;

        public double VerticalThickness =>
            Top + Bottom;

        internal bool IsDefault =>
            // With C# 7.3, Visual Studio for Mac 7.8 Preview
            (Left, Top, Right, Bottom) == (0, 0, 0, 0);

        public PILThickness(double uniformSize) : this(uniformSize, uniformSize, uniformSize, uniformSize)
        {
        }

        public PILThickness(double horizontalSize, double verticalSize) : this(horizontalSize, verticalSize, horizontalSize, verticalSize)
        {
        }

        public PILThickness(double left, double top, double right, double bottom) : this()
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static implicit operator PILThickness(double uniformSize) =>
            new PILThickness(uniformSize);

        public bool Equals(PILThickness other) =>
            // With C# 7.3, Visual Studio for Mac 7.8 Preview
            (Left, Top, Right, Bottom) == (other.Left, other.Top, other.Right, other.Bottom);

        public override bool Equals(object obj) =>
            (obj is PILThickness thickness) && Equals(thickness);

        public override int GetHashCode() =>
            (Left, Top, Right, Bottom).GetHashCode();

        public static bool operator ==(PILThickness left, PILThickness right) =>
            Equals(left, right);

        public static bool operator !=(PILThickness left, PILThickness right) =>
            !Equals(left, right);

        public void Deconstruct(out double left, out double top, out double right, out double bottom)
        {
            left = Left;
            top = Top;
            right = Right;
            bottom = Bottom;
        }
    }
}