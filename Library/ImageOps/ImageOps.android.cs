using System;

using Android.Graphics;
using Android.Media.Effect;

namespace PILSharp
{
    public static partial class ImageOps
    {
        static PILBitmapData PlatformGetBitmapData(byte[] imageData)
        {
            var bitmapData = new PILBitmapData();

            using (var options = new BitmapFactory.Options())
            {
                options.InJustDecodeBounds = true;
                BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

                bitmapData.Width = options.OutWidth;
                bitmapData.Height = options.OutHeight;
                switch (options.OutMimeType)
                {
                    case "image/bmp":
                        bitmapData.Format = Format.Bmp;
                        break;
                    case "image/jpeg":
                        bitmapData.Format = Format.Jpeg;
                        break;
                    case "image/jpg":
                        bitmapData.Format = Format.Jpeg;
                        break;
                    case "image/png":
                        bitmapData.Format = Format.Png;
                        break;
                    default:
                        throw new NotSupportedException("Provided image format is not supported");
                }
            }

            return bitmapData;
        }

        #region PlatformEqualize

        static byte[] PlatformEqualize(byte[] imageData, PILBitmapData bitmapData)
        {
            return EqualizeWithOpenGL(imageData, bitmapData);
        }

        // https://developer.android.com/reference/android/media/effect/EffectFactory#EFFECT_AUTOFIX
        // https://github.com/krazykira/VidEffects/blob/master/videffects/src/main/java/com/sherazkhilji/videffects//AutoFixEffect.java
        static byte[] EqualizeWithOpenGL(byte[] imageData, PILBitmapData bitmapData)
        {
            byte[] result = Array.Empty<byte>();

            using (var effectSurface = new EffectSurface(bitmapData))
            {
                result = effectSurface.DrawImage(imageData, true, EffectFactory.EffectAutofix);
            }

            return result;
        }

        // https://github.com/qhutch/RenderscriptHistogramEqualization
        //static byte[] EqualizeWithRenderScript(byte[] imageData, BitmapData bitmapData)
        //{
        //    throw new System.NotImplementedException();
        //}

        #endregion
    
        static byte[] PlatformExpand(byte[] imageData, PILBitmapData bitmapData, PILThickness border, PILColor? fill = null)
        {
            byte[] result = Array.Empty<byte>();

            using (var originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length))
            using (var resultImage = originalImage.Expand(border, fill))
            {
                result = resultImage.ToByteArray(bitmapData.Format);

                originalImage.Recycle();
                resultImage.Recycle();
            }

            return result;
        }
    }
}