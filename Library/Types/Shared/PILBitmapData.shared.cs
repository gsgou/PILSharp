namespace PILSharp
{
    class PILBitmapData
    {
        internal int Height
        {
            get;
            set;
        }

        internal Format Format
        {
            get;
            set;
        }

        internal int Width
        {
            get;
            set;
        }
    }

    enum Format
    {
        Bmp,
        Jpeg,
        Png,
    }
}