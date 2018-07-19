namespace PILSharp
{
    public static partial class ImageOps
    {
        public static BitmapData GetBitmapData(byte[] imageData) =>
            PlatformGetBitmapData(imageData);

        public static byte[] Equalize(byte[] imageData, int width, int height) =>
            PlatformEqualize(imageData, width, height);
    }
}