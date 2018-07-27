namespace PILSharp
{
    public static partial class ImageOps
    {
        public static byte[] Equalize(byte[] imageData) =>
            PlatformEqualize(imageData);

        public static byte[] Expand(byte[] imageData, PILThickness border, PILColor? fill = null) =>
            PlatformExpand(imageData, border, fill);
    }
}