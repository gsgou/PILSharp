namespace PILSharp
{
    public static partial class ImageOps
    {
        public static byte[] Crop(byte[] imageData, PILThickness border) =>
            PlatformCrop(imageData, border);

        public static byte[] Equalize(byte[] imageData) =>
            PlatformEqualize(imageData);

        public static byte[] Expand(byte[] imageData, PILThickness border, PILColor? fill = null) =>
            PlatformExpand(imageData, border, fill);

        public static byte[] Fit(byte[] imageData, int dstWidth, bool shouldAntialias = true) =>
            PlatformFit(imageData, dstWidth, shouldAntialias);
    }
}