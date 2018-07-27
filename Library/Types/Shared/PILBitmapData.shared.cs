namespace PILSharp
{
    public class PILBitmapData
    {
        public int Height
        {
            get;
            set;
        }

        public Format Format
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }
    }

    public enum Format
    {
        Bmp,
        Jpeg,
        Png,
    }
}