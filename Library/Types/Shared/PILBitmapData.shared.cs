namespace PILSharp
{
    public class PILBitmapData
    {
        public int Height
        {
            get;
            set;
        }

        public PILImageFormat ImageFormat
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

    public enum PILImageFormat
    {
        Bmp,
        Jpeg,
        Png,
    }
}