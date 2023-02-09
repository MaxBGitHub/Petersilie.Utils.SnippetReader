using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
namespace Petersilie.Utils.SnippetReader.OCR
{
    internal static class ImageToText
    {
        const int BIT_PER_BYTE = 8;


        private static readonly Func<Bitmap, int> GetBitPerPixel = (bmp) =>
        {
            return Bitmap.GetPixelFormatSize(bmp.PixelFormat) / BIT_PER_BYTE;
        };


        private static readonly Func<Bitmap, ImageLockMode, BitmapData> GetImageData = (bmp, mode) =>
        {
            if (bmp == null) {
                return null;
            }

            try {
                var data = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    mode, bmp.PixelFormat
                );
                return data;
            }
            catch {
                return null;
            }
        };


        private static Bitmap ApplyThreshold(int threshold, Bitmap bmp)
        {
            Bitmap dst = null;
            BitmapData imageData = null;
            BitmapData dstData = null;
            try
            {
                dst = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);
                imageData = GetImageData(bmp, ImageLockMode.ReadOnly);
                dstData = GetImageData(dst, ImageLockMode.WriteOnly);
                int bpp = GetBitPerPixel(bmp);

                unsafe
                {
                    byte* ptStride = (byte*)imageData.Scan0;
                    byte* ptDstStride = (byte*)dstData.Scan0;
                    Parallel.For(0, imageData.Height, y =>
                    {
                        byte* ptPixel = ptStride + (y * imageData.Stride);
                        byte* ptDstPixel = ptDstStride + (y * dstData.Stride);
                        for (int x = 0; x < imageData.Stride; x+= bpp) {
                            ptDstPixel[x + 2] = (byte)(ptPixel[x+2] <= threshold ? 255 : 0);
                            ptDstPixel[x + 1] = (byte)(ptPixel[x+1] <= threshold ? 255 : 0);
                            ptDstPixel[x    ] = (byte)(ptPixel[x] <= threshold ? 255 : 0);
                        }
                    });
                }
            }
            finally
            {
                if (imageData != null) {
                    bmp.UnlockBits(imageData);
                }

                if (dstData != null && dst != null) {
                    dst.UnlockBits(dstData);
                }
            }
            return dst;
        }



        private static Bitmap ApplyAdaptiveThreshold(Bitmap bmp, uint blockSize = 3)
        {
            if (blockSize == 1) {
                throw new ArgumentException("Block size has to be greater than 1.");
            }

            if (blockSize % 2 == 0) {
                throw new ArgumentException("Block size cannot be even.");
            }

            Bitmap dst = null;
            BitmapData imageData = null;
            BitmapData dstData = null;
            try
            {
                dst = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);
                imageData = GetImageData(bmp, ImageLockMode.ReadOnly);
                dstData = GetImageData(dst, ImageLockMode.WriteOnly);
                int bpp = GetBitPerPixel(bmp);

                unsafe
                {
                    byte* ptStride = (byte*)imageData.Scan0;
                    byte* ptDstStride = (byte*)dstData.Scan0;

                    
                }
            }
            catch
            {
                if (imageData != null) {
                    bmp.UnlockBits(imageData);
                }

                if (dst != null && dstData != null) {
                    dst.UnlockBits(dstData);
                }
            }
            return null;
        }


        private static int GetGlobalThreshold(int[] histogram, Bitmap bmp)
        {
            int nBytes;
            byte[] buffer;
            BitmapData imageData = null;

            try {
                imageData = GetImageData(bmp, ImageLockMode.ReadOnly);
                nBytes = imageData.Stride * imageData.Height;
                buffer = new byte[nBytes];

                Marshal.Copy(imageData.Scan0, buffer, 0, nBytes);
            }
            finally {
                if (imageData != null) {
                    bmp.UnlockBits(imageData);
                }                
            }

            int[] converted = buffer.Select(x => (int)x).ToArray();
            buffer = null;

            int init = converted.Sum() / nBytes;
            int delta = 1;

            while (delta > 0)
            {
                int mean1 = 0;
                int mean2 = 0;
                int sum1 = 0;
                int sum2 = 0;

                for (int i = 0; i < 255; i++)
                {
                    if (i <= init)
                    {
                        mean1 += histogram[i] * i;
                        sum1 += histogram[i];
                    }
                    else
                    {
                        mean2 += histogram[i] * i;
                        sum2 += histogram[i];
                    }
                }

                mean1 /= sum1;
                mean2 /= sum2;
                delta = init;
                init = (mean1 + mean2) / 2;
                delta = Math.Abs(delta - init);
            }
            return init;
        }


        private static int[] GetHistogram(Bitmap bmp)
        {
            BitmapData imageData = null;
            try
            {
                imageData = GetImageData(bmp, ImageLockMode.ReadOnly);
                
                object histLock = new object();
                int[] hist = new int[256];
                unsafe
                {
                    byte* ptStride = (byte*)imageData.Scan0;
                    Parallel.For(0, imageData.Height, y => 
                    {
                        byte* ptPixel = ptStride + (y * imageData.Stride);
                        for (int x = 0; x < imageData.Stride; x++) 
                        {
                            lock (histLock)  {
                                hist[ptPixel[x + 2]]++;
                                hist[ptPixel[x + 1]]++;
                                hist[ptPixel[x    ]]++;
                            }
                        }
                    });
                }
                return hist;
            }
            finally
            {
                if (imageData != null) {
                    bmp.UnlockBits(imageData);
                }
            }
        }


        private static Bitmap Get8BitGrayscale(Bitmap bmp)
        {
            const double YUV_FACT_R = 0.299;
            const double YUV_FACT_G = 0.587;
            const double YUV_FACT_B = 0.114;

            Bitmap dst = new Bitmap(
                bmp.Width, bmp.Height, 
                PixelFormat.Format8bppIndexed
            );

            var palette = dst.Palette;
            for (int i=0; i<256; i++) {
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            dst.Palette = palette;

            BitmapData srcData = null;
            BitmapData dstData = null;
            try
            {
                srcData = GetImageData(bmp, ImageLockMode.ReadOnly);
                dstData = GetImageData(dst, ImageLockMode.WriteOnly);

                int srcBpp = GetBitPerPixel(bmp);
                object dstLock = new object();

                unsafe
                {
                    byte* ptSrcStride = (byte*)srcData.Scan0;
                    byte* ptDstStride = (byte*)dstData.Scan0;
                    for (int y = 0; y < srcData.Height; y++) 
                    {
                        byte* ptSrcPixel = ptSrcStride + (y * srcData.Stride);
                        byte* ptDstPixel = ptDstStride + (y * dstData.Stride);
                        for (int x=0, xx=0; x<srcData.Stride; x+=srcBpp, xx++) 
                        {
                            byte yuvLuma = (byte)(
                                  (byte)(ptSrcPixel[x + 2] * YUV_FACT_R)
                                + (byte)(ptSrcPixel[x + 1] * YUV_FACT_G)
                                + (byte)(ptSrcPixel[x    ] * YUV_FACT_B)
                            );                        
                            ptDstPixel[xx] = yuvLuma;
                        }
                    }
                }                
            }
            finally
            {
                if (srcData != null) {
                    bmp.UnlockBits(srcData);
                }

                if (dstData != null) {
                    dst.UnlockBits(dstData);
                }
            }
            return dst;
        }


        private static byte[] LoadToMemory(Bitmap image)
        {   
            byte[] result = null;
            using (var mem = new MemoryStream()) {
                image.Save(mem, System.Drawing.Imaging.ImageFormat.Png) ;                
                result = mem.ToArray();
            }
            return result;
        }


        public static string GetText(Bitmap image)
        {
            if (image == null) {
                return string.Empty;
            }

            string language = CultureSettings.GetUILanguage();
            // Convert to 8-bit grayscale image.
            var mono = Get8BitGrayscale(image);
            // Dispose unused image now to free up memory.
            image.Dispose();
            // Get histogram of image.
            var hist = GetHistogram(mono);
            // Calculate the global mean threshold.
            int globalThreshold = GetGlobalThreshold(hist, mono);
            // Apply the global threshold to the image.
            mono = ApplyThreshold(globalThreshold, mono);
            
            //
            // TODO
            // ====
            // 1) Calculate adaptive threshold for image.
            // 2) Edge detection ???
            // 3) Remove artifacts 
            //   3.1) Boxes / Lines
            //   3.2) Images if possible
            //
            // 4) Compare amount of text extracted against method
            //    with no pre-processing and global threshold.
            //

            // Load image bytes into memory.
            byte[] imageBytes = LoadToMemory(mono);
            // Dispose image as it is not needed anymore.
            // Frees up memory.
            mono.Dispose();
            if (imageBytes.Length < 1) {
                return string.Empty;
            }

            string resultText = string.Empty;
            try
            {
                using (var engine = new TesseractEngine(
                    "./tessdata", 
                    language, 
                    EngineMode.Default))
                {
                    using (var tessImage = Pix.LoadFromMemory(imageBytes))
                    {
                        using (var page = engine.Process(tessImage))
                        {
                            resultText = page.GetText();
                            if (resultText == string.Empty) {
                                // try pre-processing.
                            }
                        }
                    }
                }
                return resultText;
            }
            catch (Exception e) 
            {                
                System.Diagnostics.Debug.WriteLine(e.Message);
                return string.Empty;
            }
        }
    }
}
