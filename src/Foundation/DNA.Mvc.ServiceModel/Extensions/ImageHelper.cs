//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DNA.Web
{
    public class ImageHelper
    {
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        public static string GetBase64(string fileName) 
        {
            byte[] imageBytes = null;

            using (var reader = new BinaryReader(File.OpenRead(fileName)))
            {
                try
                {
                    long size = reader.BaseStream.Length;
                    imageBytes = new byte[size];
                    for (long i = 0; i < size; i++)
                        imageBytes[i] = reader.ReadByte();
                }
                finally
                {
                }
            }
            return Convert.ToBase64String(imageBytes);
        }

        public static string GetBase64(Image img)
        {
            byte[] imageBytes = null;
            using (var buffer = new MemoryStream())
            {
                img.Save(buffer,ImageFormat.Jpeg);
                buffer.Position = 0;
                using (var reader = new BinaryReader(buffer))
                {
                    try
                    {
                        long size = reader.BaseStream.Length;
                        imageBytes = new byte[size];
                        for (long i = 0; i < size; i++)
                            imageBytes[i] = reader.ReadByte();
                    }
                    finally
                    {
                    }
                }
            }
            return Convert.ToBase64String(imageBytes);
        }

        public void SaveBase64As(string encodedData,string fileName) { }

        public static bool TryFromStringToImageFormat(string value, out ImageFormat result)
        {
            result = null;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            if (value.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring("image/".Length);
            }
            value = NormalizeImageFormat(value);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(ImageFormat));
            try
            {
                result = (ImageFormat)converter.ConvertFromInvariantString(value);
            }
            catch (NotSupportedException)
            {
                return false;
            }
            return true;
        }

        public static string NormalizeImageFormat(string value)
        {
            value = value.ToLowerInvariant();
            switch (value)
            {
                case "jpeg":
                case "jpg":
                case "pjpeg":
                    return "jpeg";

                case "png":
                case "x-png":
                    return "png";

                case "icon":
                case "ico":
                    return "icon";
            }
            return value;
        }

        public static ImageFormat GetImageFormat(string format)
        {
            ImageFormat format2;
            if (!TryFromStringToImageFormat(format, out format2))
            {
                throw new ArgumentException(string.Format("Incorrect Image Format {0}", format), "format");
            }
            return format2;
        }

        public static Image ResizeImage(Image image, int height, int width, bool preserveAspectRatio = true, bool preventEnlarge = false)
        {
            if (preserveAspectRatio)
            {
                double num3 = (height * 100.0) / ((double)image.Height);
                double num4 = (width * 100.0) / ((double)image.Width);
                if (num3 > num4)
                {
                    height = (int)Math.Round((double)((num4 * image.Height) / 100.0));
                }
                else if (num3 < num4)
                {
                    width = (int)Math.Round((double)((num3 * image.Width) / 100.0));
                }
            }

            if (preventEnlarge)
            {
                if (height > image.Height)
                {
                    height = image.Height;
                }
                if (width > image.Width)
                {
                    width = image.Width;
                }
            }
            if ((image.Height == height) && (image.Width == width))
            {
                return image;
            }
            return GetBitmapFromImage(image, width, height, true);
        }

        public static Bitmap GetBitmapFromImage(Image image, int width, int height, bool preserveResolution = true)
        {
            bool flag = (((image.PixelFormat == PixelFormat.Format1bppIndexed) || (image.PixelFormat == PixelFormat.Format4bppIndexed)) || (image.PixelFormat == PixelFormat.Format8bppIndexed)) || (image.PixelFormat == PixelFormat.Indexed);
            Bitmap bitmap = flag ? new Bitmap(width, height) : new Bitmap(width, height, image.PixelFormat);
            if (preserveResolution)
            {
                bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            }
            else
            {
                bitmap.SetResolution(96f, 96f);
            }
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                if (flag)
                {
                    graphics.FillRectangle(Brushes.Transparent, 0, 0, width, height);
                }
                //graphics.InterpolationMode = InterpolationMode.High;
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return bitmap;
        }
    }
}