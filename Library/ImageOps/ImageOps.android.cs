using System;

using Android.Graphics;
namespace PILSharp
{
    public static partial class ImageOps
    {
        static BitmapData PlatformGetBitmapData(byte[] imageData)
        {
            var bitmapData = new BitmapData();

            using (var options = new BitmapFactory.Options())
            {
                options.InJustDecodeBounds = true;
                BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

                bitmapData.Width = options.OutWidth;
                bitmapData.Height = options.OutHeight;
                switch (options.OutMimeType)
                {
                    case "image/bmp":
                        bitmapData.ImageFormat = ImageFormat.Bmp;
                        break;
                    case "image/jpeg":
                        bitmapData.ImageFormat = ImageFormat.Jpeg;
                        break;
                    case "image/jpg":
                        bitmapData.ImageFormat = ImageFormat.Jpeg;
                        break;
                    case "image/png":
                        bitmapData.ImageFormat = ImageFormat.Png;
                        break;
                    default:
                        throw new NotSupportedException("Provided image format is not supported");
                }
            }

            return bitmapData;
        }

        static byte[] PlatformEqualize(byte[] imageData, int imageWidth, int imageHeight)
        {
            return EqualizeWithOpenGL(imageData, imageWidth, imageHeight);
        }

        // https://developer.android.com/reference/android/media/effect/EffectFactory#EFFECT_AUTOFIX
        // https://github.com/krazykira/VidEffects/blob/master/videffects/src/main/java/com/sherazkhilji/videffects//AutoFixEffect.java
        static byte[] EqualizeWithOpenGL(byte[] imageData, int imageWidth, int imageHeight)
        {
            byte[] result;

            using (var outputSurface = new OutputSurface(imageWidth, imageHeight))
            {
                result = outputSurface.DrawImage(imageData);
            }

            return result;
        }

        // https://github.com/qhutch/RenderscriptHistogramEqualization
        static byte[] EqualizeWithRenderScript(byte[] imageData, int imageWidth, int imageHeight)
        {
            throw new System.NotImplementedException();
        }
    }
}