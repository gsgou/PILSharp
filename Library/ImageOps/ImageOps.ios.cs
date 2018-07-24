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
        static BitmapData PlatformGetBitmapData(byte[] imageData)
        {
            var bitmapData = new BitmapData();

            using (var cgImage = CGImageFromByteArray(imageData))
            {
                bitmapData.Width = (int)cgImage.Width;
                bitmapData.Height = (int)cgImage.Height;
                switch (cgImage.UTType)
                {
                    case "com.microsoft.bmp":
                        bitmapData.ImageFormat = ImageFormat.Bmp;
                        break;
                    case "public.jpeg":
                        bitmapData.ImageFormat = ImageFormat.Jpeg;
                        break;
                    case "public.png":
                        bitmapData.ImageFormat = ImageFormat.Png;
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

        static byte[] PlatformEqualize(byte[] imageData, int width, int height)
        {
            const int bytesPerPixel = 4;
            const int bitsPerComponent = 8;
            const CGBitmapFlags flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Big;
            int bytesPerRow = bytesPerPixel * width;

            // Convert UIImage to array of bytes in ARGB8888 pixel format
            byte[] srcArray = new byte[bytesPerRow * height];

            var uiImage = UIImageFromByteArray(imageData);

            string imageFormat = null;
            string imageFormatType = uiImage?.CGImage.UTType;
            switch (imageFormatType)
            {
                case "com.microsoft.bmp":
                    //imageFormat = "BMP";
                    imageFormat = "JPEG";
                    break;
                case "public.jpeg":
                    imageFormat = "JPEG";
                    break;
                case "public.png":
                    imageFormat = "PNG";
                    break;
                default:
                    throw new NotSupportedException($"Provided image format \"{imageFormatType}\" is not supported");
            }

            var srcContext = new CGBitmapContext(srcArray,
                                 width,
                                 height,
                                 bitsPerComponent,
                                 bytesPerRow,
                                 CGColorSpace.CreateDeviceRGB(),
                                 flags);

            srcContext?.DrawImage(new CGRect(0, 0, width, height), uiImage.CGImage);
            srcContext?.Dispose();

            var srcArrayPtr = IntPtr.Zero;
            var srcArrayHandle = GCHandle.Alloc(srcArray, GCHandleType.Pinned);
            if (srcArrayHandle.IsAllocated)
            {
                srcArrayPtr = srcArrayHandle.AddrOfPinnedObject();
            }

            var destArrayPtr = IntPtr.Zero;
            var destArray = new byte[bytesPerRow * height];
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
                Height = height,
                Width = width,
                BytesPerRow = bytesPerRow
            };

            var dest = new vImageBuffer
            {
                Data = destArrayPtr,
                Height = height,
                Width = width,
                BytesPerRow = bytesPerRow
            };

            // https://github.com/YuAo/Vivid/blob/master/Sources/YUCIHistogramEqualization.m
            vImageError err = EqualizationARGB8888(ref src, ref dest, vImageFlags.NoFlags);
            if (err != vImageError.NoError)
            {
                Console.WriteLine("vImageEqualization_ARGB8888 returned error code {0}", err.ToString());
                return Array.Empty<byte>();
            }

            // TODO:
            // Contrast Limited Adaptive Histogram Equalization
            // https://github.com/YuAo/Vivid/blob/master/Sources/YUCICLAHE.m

            CGImage destCGImage = null;
            CGBitmapContext destContext = new CGBitmapContext(destArrayPtr,
                                              width,
                                              height,
                                              bitsPerComponent,
                                              bytesPerRow,
                                              CGColorSpace.CreateDeviceRGB(),
                                              flags);
            destCGImage = destContext.ToImage();
            destContext.Dispose();

            UIImage destImage = new UIImage(destCGImage);
            NSData destData;
            switch (imageFormat)
            {
                case "com.microsoft.bmp":
                    destData = destImage.AsJPEG();
                    // BMP
                    // https://stackoverflow.com/questions/25126772/
                    // https://stackoverflow.com/questions/28648974/
                    // https://www.davidbritch.com/2015/12/accessing-image-pixel-data-in.html
                    // http://christian-helle.blogspot.com/2016/09/working-with-native-bitmap-pixel.html
                    break;
                case "JPEG":
                    destData = destImage.AsJPEG();
                    break;
                case "PNG":
                    destData = destImage.AsPNG();
                    break;
                default:
                    throw new NotSupportedException($"Provided image format \"{imageFormat}\" is not supported");
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

            return destData.ToArray();
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
    }
}