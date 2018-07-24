namespace PILSharp
{
    public class BitmapData
    {
        public int Height
        {
            get;
            set;
        }

        public ImageFormat ImageFormat
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

    public enum ImageFormat
    {
        Bmp,
        Jpeg,
        Png,
    }
}