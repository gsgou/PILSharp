namespace PILSharp
{
    public static partial class ImageOps
    {
        static BitmapData PlatformGetBitmapData(byte[] imageData) =>
            throw new NotImplementedInReferenceAssemblyException();

        static byte[] PlatformEqualize(byte[] imageData, int width, int height) =>
            throw new NotImplementedInReferenceAssemblyException();
    }
}