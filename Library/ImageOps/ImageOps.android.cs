using System;

using Android.Graphics;
using Android.Media.Effect;

namespace PILSharp
{
    public static partial class ImageOps
    {
        static PILBitmapData GetPILBitmapData(byte[] imageData)
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

        #region PlatformCrop

        static byte[] PlatformCrop(byte[] imageData, PILThickness border)
        {
            byte[] result = Array.Empty<byte>();

            PILBitmapData bitmapData = GetPILBitmapData(imageData);
            int cropWidth = bitmapData.Width - (int)border.HorizontalThickness;
            int cropHeight = bitmapData.Height - (int)border.VerticalThickness;
            if (cropWidth < 0 || cropHeight < 0)
            {
                throw new ArgumentException("PILThickness is not valid.");
            }

            using (var originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length))
            using (var resultImage = Bitmap.CreateBitmap(originalImage,
                                                         (int)border.Left,
                                                         (int)border.Top,
                                                         cropWidth,
                                                         cropHeight))
            {
                result = resultImage.ToByteArray(bitmapData.Format);

                originalImage.Recycle();
                resultImage.Recycle();
            }

            return result;
        }

        #endregion

        #region PlatformEqualize

        static byte[] PlatformEqualize(byte[] imageData)
        {
            return EqualizeWithOpenGL(imageData);
        }

        // https://developer.android.com/reference/android/media/effect/EffectFactory#EFFECT_AUTOFIX
        // https://github.com/krazykira/VidEffects/blob/master/videffects/src/main/java/com/sherazkhilji/videffects//AutoFixEffect.java
        static byte[] EqualizeWithOpenGL(byte[] imageData)
        {
            byte[] result = Array.Empty<byte>();

            PILBitmapData bitmapData = GetPILBitmapData(imageData);
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

        #region PlatformExpand

        static byte[] PlatformExpand(byte[] imageData, PILThickness border, PILColor? fill = null)
        {
            byte[] result = Array.Empty<byte>();

            using (var originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length))
            using (var resultImage = originalImage.Expand(border, fill))
            {
                PILBitmapData bitmapData = GetPILBitmapData(imageData);
                result = resultImage.ToByteArray(bitmapData.Format);

                originalImage.Recycle();
                resultImage.Recycle();
            }

            return result;
        }

        #endregion
    
        #region PlatformFit

        static byte[] PlatformFit(byte[] imageData, int dstWidth, bool shouldAntialias = true)
        {
            byte[] result = Array.Empty<byte>();

            using (var originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length))
            using (var resultImage = originalImage.Fit(dstWidth, shouldAntialias))
            {
                PILBitmapData bitmapData = GetPILBitmapData(imageData);
                result = resultImage.ToByteArray(bitmapData.Format);
            }

            return result;
        }

        #endregion
    }
}