namespace PILSharp
{
    public static partial class ImageOps
    {
        public static PILBitmapData GetBitmapData(byte[] imageData) =>
            PlatformGetBitmapData(imageData);

        public static byte[] Equalize(byte[] imageData, PILBitmapData bitmapData) =>
            PlatformEqualize(imageData, bitmapData);

        public static byte[] Expand(byte[] imageData, PILBitmapData bitmapData, PILThickness border, PILColor? fill = null) =>
            PlatformExpand(imageData, bitmapData, border, fill);
    }
}