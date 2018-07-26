namespace PILSharp
{
    public static partial class ImageOps
    {
        static PILBitmapData PlatformGetBitmapData(byte[] imageData) =>
            throw new NotImplementedInReferenceAssemblyException();

        static byte[] PlatformEqualize(byte[] imageData, PILBitmapData bitmapData) =>
            throw new NotImplementedInReferenceAssemblyException();

        static byte[] PlatformExpand(byte[] imageData, PILBitmapData bitmapData, PILThickness border, PILColor? fill = null) =>
            throw new NotImplementedInReferenceAssemblyException();
    }
}