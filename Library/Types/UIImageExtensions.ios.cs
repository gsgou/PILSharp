using System;
using System.Runtime.InteropServices;

using CoreGraphics;
using UIKit;

namespace PILSharp
{
    public static class UIImageExtensions
    {
        internal static byte[] AsBMP(this UIImage uiImage)
        {
            if (uiImage == null)
            {
                return null;
            }

            int width = (int)uiImage.Size.Width;
            int height = (int)uiImage.Size.Height;
            const int pixelDataOffset = 54;
            uint rawPixelDataSize = (uint)width * (uint)height * 4;
            uint size = rawPixelDataSize + 14 + 40;

            // Data needs to be BGRA
            byte[] result = new byte[size];
            result[0] = 0x42; result[1] = 0x4D;     // BITMAPFILEHEADER "BM"
            SetLong(result, 0x2, size);             // File size
            SetLong(result, 0xA, pixelDataOffset);  // Offset to pixel data
            SetLong(result, 0xE, 40);               // Bytes in DIB header (BITMAPINFOHEADER)
            SetLong(result, 0x12, (uint)width);
            SetLong(result, 0x16, (uint)height);
            SetShort(result, 0x1A, 1);              // 1 plane
            SetShort(result, 0x1C, 32);             // 32 bits
            SetLong(result, 0x22, rawPixelDataSize);
            SetLong(result, 0x26, 2835);            // h/v pixels per meter device resolution
            SetLong(result, 0x2A, 2835);

            byte[] buffer = new byte[width * height * 4];
            var handle = GCHandle.Alloc(buffer);
            try
            {
                using (var colorSpace = CGColorSpace.CreateGenericRgb())
                using (var context = new CGBitmapContext(buffer,
                                         width,
                                         height,
                                         8,
                                         4 * width,
                                         colorSpace,
                                         CGImageAlphaInfo.PremultipliedLast))
                using (var cgImage = uiImage.CGImage)    
                {
                    // Draw the RGBA image
                    context.DrawImage(new CGRect(0, 0, width, height), cgImage);
                }

                int di = pixelDataOffset;
                int si;
                for (int y = 0; y < height; y++)
                {
                    si = (height - y - 1) * 4 * width;
                    for (int x = 0; x < width; x++)
                    {
                        CopyFlipPixel(buffer, si, result, di);
                        di += 4; // Destination marchs forward
                        si += 4;
                    }
                }
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        static void CopyFlipPixel(byte[] src, int srcOffset, byte[] dst, int dstOffset)
        {
            int s = srcOffset;
            int d = dstOffset + 2;
            dst[d--] = src[s++];          // R
            dst[d--] = src[s++];          // G
            dst[d--] = src[s++];          // B
            dst[dstOffset + 3] = src[s];  // Alpha
        }

        static void SetShort(byte[] src, int offset, UInt16 v)
        {
            var bytes = BitConverter.GetBytes(v);
            if (!BitConverter.IsLittleEndian)
            { 
                Array.Reverse(bytes); 
            }
            Array.Copy(bytes, 0, src, offset, bytes.Length);
        }

        static void SetLong(byte[] src, int offset, UInt32 v)
        {
            var bytes = BitConverter.GetBytes(v);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            Array.Copy(bytes, 0, src, offset, bytes.Length);
        }
    }
}