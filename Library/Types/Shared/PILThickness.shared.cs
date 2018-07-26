namespace PILSharp
{
    public struct PILThickness
    {
        // Try to match Xamarin.Forms:
        // https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/Thickness.cs

        public double Left { get; set; }

        public double Top { get; set; }

        public double Right { get; set; }

        public double Bottom { get; set; }

        public double HorizontalThickness
        {
            get { return Left + Right; }
        }

        public double VerticalThickness
        {
            get { return Top + Bottom; }
        }

        internal bool IsDefault
        {
            get { return Left == 0 && Top == 0 && Right == 0 && Bottom == 0; }
        }

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

        public static implicit operator PILThickness(double uniformSize)
        {
            return new PILThickness(uniformSize);
        }

        bool Equals(PILThickness other)
        {
            return Left.Equals(other.Left) && Top.Equals(other.Top) && Right.Equals(other.Right) && Bottom.Equals(other.Bottom);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is PILThickness && Equals((PILThickness)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Left.GetHashCode();
                hashCode = (hashCode * 397) ^ Top.GetHashCode();
                hashCode = (hashCode * 397) ^ Right.GetHashCode();
                hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator == (PILThickness left, PILThickness right)
        {
            return left.Equals(right);
        }

        public static bool operator != (PILThickness left, PILThickness right)
        {
            return !left.Equals(right);
        }

        public void Deconstruct(out double left, out double top, out double right, out double bottom)
        {
            left = Left;
            top = Top;
            right = Right;
            bottom = Bottom;
        }
    }
}