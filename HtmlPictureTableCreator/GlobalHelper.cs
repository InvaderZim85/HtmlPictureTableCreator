using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace HtmlPictureTableCreator
{
    public static class GlobalHelper
    {
        /// <summary>
        /// Contains the different info types
        /// </summary>
        public enum InfoType
        {
            /// <summary>
            /// A normal info message
            /// </summary>
            Info,
            /// <summary>
            /// An error message
            /// </summary>
            Error
        }
        /// <summary>
        /// The different image ratios
        /// </summary>
        public enum ImageRatio
        {
            /// <summary>
            /// Custom ratio
            /// </summary>
            [Description("Custom")]
            Custom = 0,
            /// <summary>
            /// 4:3 ratio
            /// </summary>
            [Description("4:3")]
            FourToThree = 1,
            /// <summary>
            /// 16:9 ratio
            /// </summary>
            [Description("16:9")]
            SixteenToNine = 2,
            /// <summary>
            /// 16:10 ratio
            /// </summary>
            [Description("16:10")]
            SixteenToTen = 3
        }

        /// <summary>
        /// Delegate for an info message event
        /// </summary>
        /// <param name="infoType">The type of the message (<see cref="InfoType"/>)</param>
        /// <param name="message">The message</param>
        public delegate void InfoEvent(InfoType infoType, string message);
        /// <summary>
        /// Delegate for the progress event
        /// </summary>
        /// <param name="value">The current value</param>
        /// <param name="maxValue">The max value</param>
        public delegate void ProgressEvent(double value, double maxValue);

        /// <summary>
        /// Contains the different file types
        /// </summary>
        private static readonly string[] FileTypes = { ".jpeg", ".jpg", ".png", ".bmp" };

        /// <summary>
        /// Loads the image files which are stored in the given folder
        /// </summary>
        /// <param name="path">The path of the folder</param>
        /// <returns>The image files</returns>
        public static List<FileInfo> GetImageFiles(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var dirInfo = new DirectoryInfo(path);
            var tmpFiles = dirInfo.GetFiles();

            return tmpFiles.Where(w => FileTypes.Contains(w.Extension.ToLower())).ToList();
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="imageFile">The <see cref="FileInfo"/> object of the image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static Bitmap ResizeImage(FileInfo imageFile, int width, int height)
        {
            if (imageFile == null)
                throw new ArgumentNullException(nameof(imageFile));

            if (width == 0)
                throw new ArgumentException("A width of 0 is not supported.");

            if (height == 0)
                throw new ArgumentException("A height of 0 is not supported.");

            var image = Image.FromFile(imageFile.FullName);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        /// <summary>
        /// Extracts the description of an enum
        /// </summary>
        /// <param name="value">The enum</param>
        /// <returns>The description</returns>
        public static string GetEnumDescription(Enum value)
        {
            try
            {
                var fieldInfo = value.GetType().GetField(value.ToString());

                var attributes =
                    (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return attributes.Length > 0 ? attributes[0].Description : value.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("An error has occured while extracting the enum description.", ex);
                return value?.ToString() ?? "";
            }
        }

        /// <summary>
        /// Calculates the progress
        /// </summary>
        /// <param name="step">The current step</param>
        /// <param name="max">The max step</param>
        /// <returns>The percentage value</returns>
        public static double CalculateCurrentProgress(double step, double max)
        {
            if (max == 0)
                return 0;

            return 100 / max * step;
        }
    }
}
