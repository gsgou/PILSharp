using System;
using System.Runtime.InteropServices;

using Accelerate;
using CoreGraphics;
using Foundation;
using ImageIO;
using UIKit;

namespace PILSharp
{
    public static partial class ImageOps
    {
        static PILBitmapData PlatformGetBitmapData(byte[] imageData)
        {
            var bitmapData = new PILBitmapData();

            using (var cgImage = CGImageFromByteArray(imageData))
            {
                bitmapData.Width = (int)cgImage.Width;
                bitmapData.Height = (int)cgImage.Height;
                switch (cgImage.UTType)
                {
                    case "com.microsoft.bmp":
                        bitmapData.ImageFormat = PILImageFormat.Bmp;
                        break;
                    case "public.jpeg":
                        bitmapData.ImageFormat = PILImageFormat.Jpeg;
                        break;
                    case "public.png":
                        bitmapData.ImageFormat = PILImageFormat.Png;
                        break;
                    default:
                        throw new NotSupportedException("Provided image format is not supported");
                }
            }
                   
            return bitmapData;
        }

        const string AccelerateImageLibrary = "/System/Library/Frameworks/Accelerate.framework/Frameworks/vImage.framework/vImage";
        [DllImport(AccelerateImageLibrary)]
        extern static nint vImageEqualization_ARGB8888(ref vImageBuffer src, ref vImageBuffer dest, vImageFlags flags);
        unsafe static vImageError EqualizationARGB8888(ref vImageBuffer src, ref vImageBuffer dest, vImageFlags flags)
        {
            return (vImageError)(long)vImageEqualization_ARGB8888(ref src, ref dest, flags);
        }

        static byte[] PlatformEqualize(byte[] imageData, PILBitmapData bitmapData)
        {
            const int bytesPerPixel = 4;
            const int bitsPerComponent = 8;
            const CGBitmapFlags flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Big;
            int bytesPerRow = bytesPerPixel * bitmapData.Width;

            // Convert UIImage to array of bytes in ARGB8888 pixel format
            byte[] srcArray = new byte[bytesPerRow * bitmapData.Height];

            using (var colorSpace = CGColorSpace.CreateGenericRgb())
            using (var srcContext = new CGBitmapContext(srcArray,
                                     bitmapData.Width,
                                     bitmapData.Height,
                                     bitsPerComponent,
                                     bytesPerRow,
                                     colorSpace,
                                     flags))
            using (var uiImage = UIImageFromByteArray(imageData))
            using (var cgImage = uiImage.CGImage)
            {
                srcContext.DrawImage(new CGRect(0, 0, bitmapData.Width, bitmapData.Height), cgImage);
            }

            var srcArrayPtr = IntPtr.Zero;
            var srcArrayHandle = GCHandle.Alloc(srcArray, GCHandleType.Pinned);
            if (srcArrayHandle.IsAllocated)
            {
                srcArrayPtr = srcArrayHandle.AddrOfPinnedObject();
            }

            var destArrayPtr = IntPtr.Zero;
            var destArray = new byte[bytesPerRow * bitmapData.Height];
            var destArrayHandle = GCHandle.Alloc(destArray, GCHandleType.Pinned);
            if (destArrayHandle.IsAllocated)
            {
                destArrayPtr = destArrayHandle.AddrOfPinnedObject();
            }

            if (srcArrayPtr == IntPtr.Zero || destArrayPtr == IntPtr.Zero)
            {
                return Array.Empty<byte>();
            }

            var src = new vImageBuffer
            {
                Data = srcArrayPtr,
                Height = bitmapData.Height,
                Width = bitmapData.Width,
                BytesPerRow = bytesPerRow
            };

            var dest = new vImageBuffer
            {
                Data = destArrayPtr,
                Height = bitmapData.Height,
                Width = bitmapData.Width,
                BytesPerRow = bytesPerRow
            };

            // https://github.com/YuAo/Vivid/blob/master/Sources/YUCIHistogramEqualization.m
            // https://developer.apple.com/documentation/accelerate/vimage/vimage_operations/histogram
            // TODO:
            // Contrast Limited Adaptive Histogram Equalization
            // https://github.com/YuAo/Vivid/blob/master/Sources/YUCICLAHE.m
            vImageError err = EqualizationARGB8888(ref src, ref dest, vImageFlags.NoFlags);
            if (err != vImageError.NoError)
            {
                Console.WriteLine("vImageEqualization_ARGB8888 returned error code {0}", err.ToString());
                return Array.Empty<byte>();
            }

            byte[] destDataArray = Array.Empty<byte>();

            using (var colorSpace = CGColorSpace.CreateGenericRgb())
            using (var destContext = new CGBitmapContext(destArrayPtr,
                                         bitmapData.Width,
                                         bitmapData.Height,
                                         bitsPerComponent,
                                         bytesPerRow,
                                         colorSpace,
                                         flags))
            using (var destCGImage = destContext.ToImage())
            using (var destImage = new UIImage(destCGImage))
            {
                destDataArray = destImage.ToByteArray(bitmapData.ImageFormat);
            }

            if (srcArrayHandle.IsAllocated)
            {
                srcArrayHandle.Free();
                srcArrayPtr = IntPtr.Zero;
            }

            if (destArrayHandle.IsAllocated)
            {
                destArrayHandle.Free();
                destArrayPtr = IntPtr.Zero;
            }

            return destDataArray;
        }

        static UIImage UIImageFromByteArray(byte[] data)
        {
            if (!(data?.Length > 0))
            {
                return null;
            }

            UIImage image = null;
            try
            {
                image = new UIImage(NSData.FromArray(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Image load failed: " + ex.Message);
            }
            return image;
        }

        static CGImage CGImageFromByteArray(byte[] data)
        {
            if (!(data?.Length > 0))
            {
                return null;
            }

            CGImage image = null;
            using (var imageSource = CGImageSource.FromData(NSData.FromArray(data)))
            {
                var decodeOptions = new CGImageOptions
                {
                    ShouldAllowFloat = false,
                    ShouldCache = false
                };
                image = imageSource.CreateImage(0, decodeOptions);
            }

            return image;
        }
    
        static byte[] PlatformExpand(byte[] imageData, PILBitmapData bitmapData, PILThickness border, PILColor? fill = null)
        {
            byte[] result = Array.Empty<byte>();

            using (var originalImage = UIImageFromByteArray(imageData))
            using (var expandedImage = originalImage.Expand(border, fill))
            {
                result = expandedImage.ToByteArray(bitmapData.ImageFormat);
            }

            return result;
        }
    }
}