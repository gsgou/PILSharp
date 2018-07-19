using Android.Graphics;

namespace PILSharp
{
    public static partial class ImageOps
    {
        static BitmapData PlatformGetBitmapData(byte[] imageData)
        {
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            var bitmapData = new BitmapData();
            bitmapData.Width = bitmap.Width;
            bitmapData.Height = bitmap.Height;
            bitmapData.Stride = bitmap.RowBytes;
            //bitmapData.Stride = (int)bitmap.GetBitmapInfo().Stride;

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