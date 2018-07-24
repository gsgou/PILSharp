namespace PILSharp
{
    public static partial class ImageOps
    {
        public static BitmapData GetBitmapData(byte[] imageData) =>
            PlatformGetBitmapData(imageData);

        public static byte[] Equalize(byte[] imageData, BitmapData bitmapData) =>
            PlatformEqualize(imageData, bitmapData);
    }
}