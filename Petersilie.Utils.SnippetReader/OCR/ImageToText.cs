using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Tesseract;
namespace Petersilie.Utils.SnippetReader.OCR
{
    internal static class ImageToText
    {
        private static double[] GetHistogram(Bitmap bmp)
        {
            BitmapData imageData = null;
            try
            {
                imageData = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height), 
                    ImageLockMode.ReadOnly, 
                    bmp.PixelFormat
                );

                object histLock = new object();
                double[] hist = new double[256];
                unsafe
                {
                    byte* ptStride = (byte*)imageData.Scan0;
                    Parallel.For(0, imageData.Height, y => 
                    {
                        byte* ptPixel = ptStride + (y * imageData.Stride);
                        for (int x = 0; x < imageData.Stride; x++) 
                        {
                            lock (histLock) 
                            {
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

        const double YUV_FACT_R = 0.299;
        const double YUV_FACT_G = 0.587;
        const double YUV_FACT_B = 0.114;

        [Obsolete("Use Get8BitGrayscale(Bitmap) instead")]
        private static void ApplyGrayscale(ref Bitmap bmp)
        {
            BitmapData imageData = null;
            try
            {
                imageData = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite,
                    bmp.PixelFormat
                );

                unsafe
                {
                    int bpp = Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
                    byte * ptStride = (byte*)imageData.Scan0;
                    Parallel.For(0, imageData.Height, y => {
                        byte* ptPixel = ptStride + (y * imageData.Stride);
                        for (int x=0; x< imageData.Stride; x+=bpp) {
                            byte yuvLuma = (byte)(
                                  (byte)(ptPixel[x + 2] * YUV_FACT_R)
                                + (byte)(ptPixel[x + 1] * YUV_FACT_G)
                                + (byte)(ptPixel[x    ] * YUV_FACT_B)
                            );
                            ptPixel[x    ] = yuvLuma;
                            ptPixel[x + 1] = yuvLuma;
                            ptPixel[x + 2] = yuvLuma;
                        }
                    });
                }
            }
            finally
            {
                if (imageData != null ) {
                    bmp.UnlockBits( imageData );
                }
            }
        }


        private static Bitmap Get8BitGrayscale(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Bitmap dst = new Bitmap(w, h, PixelFormat.Format8bppIndexed);
            var palette = dst.Palette;
            for (int i=0; i<256; i++) {
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            dst.Palette = palette;

            BitmapData srcData = null;
            BitmapData dstData = null;
            try
            {
                srcData = bmp.LockBits(
                    new Rectangle(0, 0, w, h),
                    ImageLockMode.ReadOnly,
                    bmp.PixelFormat
                );

                dstData = dst.LockBits(
                    new Rectangle(0, 0, w, h),
                    ImageLockMode.WriteOnly,
                    dst.PixelFormat
                );

                int srcBpp = Bitmap.GetPixelFormatSize(srcData.PixelFormat) / 8;
                object dstLock = new object();

                unsafe
                {
                    byte* ptSrcStride = (byte*)srcData.Scan0;
                    byte* ptDstStride = (byte*)dstData.Scan0;
                    for (int y = 0; y < h; y++) 
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

            var mono = Get8BitGrayscale(image);
            //
            // TODO
            // ====
            // Calculate global threshold for image by using histogram.
            // Apply threshold and compare amount of text extracted
            // against method with no gobal threshold.
            //
            var threshold = GetHistogram(mono);

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

            byte[] imageBytes = LoadToMemory(mono);
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
