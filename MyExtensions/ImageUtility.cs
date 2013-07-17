using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq.Expressions;

namespace MyExtensions
{
    public class ImageUtility
    {
        /// <summary>
        /// http://www.switchonthecode.com/tutorials/csharp-tutorial-image-editing-saving-cropping-and-resizing
        /// </summary>
        /// <param name="image"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image ImageResize(Image image, Size size, DominantMeasure dominantMeasure = DominantMeasure.Auto)
        {
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            #region calcolo misura dominante (larghezza o altezza)
            if (dominantMeasure == DominantMeasure.Auto)
            {
                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;
            }
            else if (dominantMeasure == DominantMeasure.Width)
                nPercent = nPercentW;
            else if (dominantMeasure == DominantMeasure.Height)
                nPercent = nPercentH;
            #endregion

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(image, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        public static Image ImageCrop(Image image, int width, int height, AnchorPosition anchor)
        {
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = (Convert.ToSingle(width) / Convert.ToSingle(sourceWidth));
            nPercentH = (Convert.ToSingle(height) / Convert.ToSingle(sourceHeight));

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                switch (anchor)
                {
                    case AnchorPosition.Top:
                        destY = 0;
                        break;
                    case AnchorPosition.Bottom:
                        destY = Convert.ToInt32(height - (sourceHeight * nPercent));
                        break;
                    default:
                        destY = Convert.ToInt32((height - (sourceHeight * nPercent)) / 2);
                        break;
                }
            }
            else
            {
                nPercent = nPercentH;
                switch (anchor)
                {
                    case AnchorPosition.Left:
                        destX = 0;
                        break;
                    case AnchorPosition.Right:
                        destX = Convert.ToInt32((width - (sourceWidth * nPercent)));
                        break;
                    default:
                        destX = Convert.ToInt32(((width - (sourceWidth * nPercent)) / 2));
                        break;
                }
            }

            int destWidth = Convert.ToInt32((sourceWidth * nPercent));
            int destHeight = Convert.ToInt32((sourceHeight * nPercent));

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.DrawImage(image, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
            grPhoto.Dispose();

            return bmPhoto;
        }

        public static Stream ImageConvert(Image img, int size, int maxFileSize)
        {
            Image resizedImage = ImageResize(img, new Size(size, size));
            Stream result = null;
            long quality = 100;
            do
            {
                result = ImageConvertToJpeg(resizedImage, quality);
                quality -= 10;
            } while (result.Length > maxFileSize);
            if (result.Length > maxFileSize)
                throw new Exception("Non è possibile convertire l'immagine: dimensione finale maggiore di " + maxFileSize);
            return result;
        }

        public static Stream ImageConvertToJpeg(Image img, long quality)
        {
            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return null;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            Stream result = new MemoryStream();
            img.Save(result, jpegCodec, encoderParams);
            result.Position = 0;
            return result;
        }

        private static ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        public enum DominantMeasure
        {
            Auto,
            Width,
            Height
        }

        public enum AnchorPosition
        {
            Top,
            Center,
            Bottom,
            Left,
            Right
        }
    }
}