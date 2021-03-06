using System;
using System.IO;

using Android.Graphics;
using Android.Runtime;
using Android.Support.V8.Renderscript;
using Element = Android.Support.V8.Renderscript.Element;
using Type = Android.Support.V8.Renderscript.Type;
using Java.Nio;

namespace PILSharp
{
    public static class BitmapExtensions
    {
        // Write integer to little-endian
        static byte[] writeInt(int value)
        {
            byte[] b = new byte[4];

            b[0] = (byte)(value & 0x000000FF);
            b[1] = (byte)((value & 0x0000FF00) >> 8);
            b[2] = (byte)((value & 0x00FF0000) >> 16);
            b[3] = (byte)((value & 0xFF000000) >> 24);

            return b;
        }

        // Write short to little-endian byte array
        static byte[] writeShort(short value)
        {
            byte[] b = new byte[2];

            b[0] = (byte)(value & 0x00FF);
            b[1] = (byte)((value & 0xFF00) >> 8);

            return b;
        }

        // Android Bitmap Object to Window's v3 24bit Bmp Format File
        internal static byte[] AsBMP(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentException();
            }

            // Image size
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Image dummy data size
            // Reason: the amount of bytes per image row must be a multiple of 4 (requirements of bmp format)
            byte[] dummyBytesPerRow = Array.Empty<byte>(); ;
            bool hasDummy = false;
            // Source image width * number of bytes to encode one pixel.
            const int BYTE_PER_PIXEL = 3;
            const int BMP_WIDTH_OF_TIMES = 4;
            int rowWidthInBytes = BYTE_PER_PIXEL * width;
            if (rowWidthInBytes % BMP_WIDTH_OF_TIMES > 0)
            {
                hasDummy = true;
                // The number of dummy bytes we need to add on each row
                dummyBytesPerRow = new byte[(BMP_WIDTH_OF_TIMES - (rowWidthInBytes % BMP_WIDTH_OF_TIMES))];
                // Just fill an array with the dummy bytes we need to append at the end of each row
                for (int i = 0; i < dummyBytesPerRow.Length; i++)
                {
                    dummyBytesPerRow[i] = (byte)0xFF;
                }
            }

            // An array to receive the pixels from the source image
            int[] pixels = new int[width * height];

            // The number of bytes used in the file to store raw image data (excluding file headers)
            int imageSize = (rowWidthInBytes + (hasDummy ? dummyBytesPerRow.Length : 0)) * height;
            // File headers size
            int imageDataOffset = 0x36;

            // Final size of the file
            int fileSize = imageSize + imageDataOffset;

            // Android Bitmap Image Data
            bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);

            ByteBuffer buffer = ByteBuffer.Allocate(fileSize);

            // BITMAP FILE HEADER Write Start
            buffer.Put((sbyte)0x42);
            buffer.Put((sbyte)0x4D);

            // Size
            buffer.Put(writeInt(fileSize));

            // Reserved
            buffer.Put(writeShort((short)0));
            buffer.Put(writeShort((short)0));

            // Image data start offset
            buffer.Put(writeInt(imageDataOffset));

            // BITMAP INFO HEADER Write Start
            // Size
            buffer.Put(writeInt(0x28));

            // width, height
            // If we add 3 dummy bytes per row it means we add a pixel and the image width is modified.
            buffer.Put(writeInt(width + (hasDummy ? (dummyBytesPerRow.Length == 3 ? 1 : 0) : 0)));
            buffer.Put(writeInt(height));

            // Planes
            buffer.Put(writeShort((short)1));

            // Bit count
            buffer.Put(writeShort((short)24));

            // Bit compression
            buffer.Put(writeInt(0));

            // Image data size
            buffer.Put(writeInt(imageSize));

            // Horizontal resolution in pixels per meter
            buffer.Put(writeInt(0));

            // Vertical resolution in pixels per meter (unreliable)
            buffer.Put(writeInt(0));

            buffer.Put(writeInt(0));

            buffer.Put(writeInt(0));

            // BITMAP INFO HEADER Write End
            int row = height;
            int col = width;
            int startPosition = (row - 1) * col;
            int endPosition = row * col;

            // This while loop is a lengthy process
            // Puts take a while so only do one by creating a big array called final
            byte[] final = Array.Empty<byte>();
            while (row > 0)
            {
                // This array is also used to cut down on time of puts
                byte[] b = new byte[(endPosition - startPosition) * 3];
                int counter = 0;
                for (int i = startPosition; i < endPosition; i++)
                {
                    b[counter] = (byte)((pixels[i] & 0x000000FF));
                    b[counter + 1] = (byte)((pixels[i] & 0x0000FF00) >> 8);
                    b[counter + 2] = (byte)((pixels[i] & 0x00FF0000) >> 16);
                    counter += 3;
                }
                int finalPriorLength = final.Length;
                Array.Resize<byte>(ref final, finalPriorLength + b.Length);
                Array.Copy(b, 0, final, finalPriorLength, b.Length);

                if (hasDummy)
                {
                    finalPriorLength = final.Length;
                    Array.Resize<byte>(ref final, finalPriorLength + dummyBytesPerRow.Length);
                    Array.Copy(dummyBytesPerRow, 0, final, finalPriorLength, dummyBytesPerRow.Length);
                }
                row--;
                endPosition = startPosition;
                startPosition = startPosition - col;
            }
            buffer.Put(final);
            buffer.Rewind();

            IntPtr classHandle = JNIEnv.FindClass("java/nio/ByteBuffer");
            IntPtr methodId = JNIEnv.GetMethodID(classHandle, "array", "()[B");
            IntPtr resultHandle = JNIEnv.CallObjectMethod(buffer.Handle, methodId);
            byte[] result = JNIEnv.GetArray<byte>(resultHandle);
            JNIEnv.DeleteLocalRef(resultHandle);
            buffer.Dispose();

            return result;
        }

        internal static byte[] ToByteArray(this Bitmap bitmap, Format imageFormat)
        {
            if (bitmap == null)
            {
                throw new ArgumentException();
            }

            byte[] result = Array.Empty<byte>();

            using (var stream = new MemoryStream())
            {
                switch (imageFormat)
                {
                    case Format.Bmp:
                        result = bitmap.AsBMP();
                        break;
                    case Format.Jpeg:
                        bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                        result = stream.ToArray();
                        break;
                    case Format.Png:
                        bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                        result = stream.ToArray();
                        break;
                }
            }

            return result;
        }

        internal static Bitmap Expand(this Bitmap bitmap, PILThickness border, PILColor? fill = null)
        {
            if (bitmap == null)
            {
                throw new ArgumentException();
            }

            if (!fill.HasValue)
            {
                // Transparent as default color
                fill = new PILColor(255, 255, 255, 0);
            }

            Bitmap resultImage = Bitmap.CreateBitmap(bitmap.Width + (int)border.HorizontalThickness,
                                                     bitmap.Height + (int)border.VerticalThickness,
                                                     Bitmap.Config.Argb8888);
            using (var canvas = new Canvas(resultImage))
            {
                canvas.DrawColor(fill.Value.ToAndroid());
                canvas.DrawBitmap(bitmap, (float)border.Left, (float)border.Top, null);
            }

            return resultImage;
        }
    
        internal static Bitmap Fit(this Bitmap bitmap, int dstWidth, bool shouldAntialias = true)
        {
            if (bitmap == null)
            {
                throw new ArgumentException();
            }

            Bitmap dstBitmap = null;

            int srcWidth = bitmap.Width;
            int srcHeight = bitmap.Height;
            float srcAspectRatio = (float)srcWidth / srcHeight;
            int dstHeight = (int)(dstWidth / srcAspectRatio);

            // Create the Renderscript context.
            RenderScript renderScript = RenderScript.Create(Platform.CurrentActivity);
            // Create an Allocation for the kernel inputs. Android.Renderscripts.AllocationUsage.Script int is 1
            Allocation input = Allocation.CreateFromBitmap(renderScript, bitmap, Allocation.MipmapControl.MipmapFull, 1);
            // Create an Allocation for the blurred output.
            Allocation blurOutput = null;

            // Alias free resize with RenderScript
            // https://medium.com/@petrakeas/alias-free-resize-with-renderscript-5bf15a86ce3
            if (shouldAntialias)
            {
                #region Calculate gaussian's radius

                float resizeRatio = (float)srcWidth / dstWidth;
                float sigma = resizeRatio / (float)Math.PI;
                // https://android.googlesource.com/platform/frameworks/rs/+/master/cpu_ref/rsCpuIntrinsicBlur.cpp
                float radius = 2.5f * sigma - 1.5f;
                radius = Math.Min(25, Math.Max(0.0001f, radius));

                #endregion

                using (var inputType = input.Type)
                using (var blurIntrinsic = ScriptIntrinsicBlur.Create(renderScript, Element.U8_4(renderScript)))
                {
                    // Assign the input Allocation to the script.
                    blurIntrinsic.SetInput(input);

                    // Set the blur radius.
                    blurIntrinsic.SetRadius(radius);

                    // We need to create an output allocation to hold the output of the Renderscript.
                    blurOutput = Allocation.CreateTyped(renderScript, inputType);

                    // This will run the script over each Element in the Allocation and copy it's output to filtered.
                    blurIntrinsic.ForEach(blurOutput);

                    // Cleanup.
                    blurIntrinsic.Destroy();
                    inputType.Destroy();
                }
            }

            using (var outputType = (shouldAntialias == true) ?
                                    Type.CreateXY(renderScript, blurOutput.Element, dstWidth, dstHeight) :
                                    Type.CreateXY(renderScript, input.Element, dstWidth, dstHeight))
            using (var output = Allocation.CreateTyped(renderScript, outputType))
            using (var resizeIntrinsic = ScriptIntrinsicResize.Create(renderScript))
            using (var bitmapConfig = bitmap.GetConfig())
            {
                if (shouldAntialias)
                {
                    resizeIntrinsic.SetInput(blurOutput);
                }
                else
                {
                    resizeIntrinsic.SetInput(input);
                }

                resizeIntrinsic.ForEach_bicubic(output);

                // Copy the output to the destination bitmap.
                dstBitmap = Bitmap.CreateBitmap(dstWidth, dstHeight, bitmapConfig);
                output.CopyTo(dstBitmap);

                // Cleanup.
                resizeIntrinsic.Destroy();
                output.Destroy();
                outputType.Destroy();
            }

            // Cleanup.
            blurOutput?.Destroy();
            blurOutput?.Dispose();
            input.Destroy();
            input.Dispose();
            renderScript.Destroy();
            renderScript.Dispose();

            return dstBitmap;
        }
    }
}