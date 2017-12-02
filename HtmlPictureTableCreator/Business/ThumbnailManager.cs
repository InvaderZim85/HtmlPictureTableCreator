using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using HtmlPictureTableCreator.DataObjects;
using HtmlPictureTableCreator.Global;

namespace HtmlPictureTableCreator.Business
{
    public static class ThumbnailManager
    {
        /// <summary>
        /// Occurs if there is a new info
        /// </summary>
        public static event GlobalHelper.InfoEvent OnNewInfo;
        /// <summary>
        /// Occurs when the progress goes on
        /// </summary>
        public static event GlobalHelper.ProgressEvent OnProgress;

        /// <summary>
        /// Contains the name of the thumbnail folder
        /// </summary>
        private const string ThumbnailFolderName = "thumbnails";
        /// <summary>
        /// Creates thumbnails for the images, wich are stored in the given source
        /// </summary>
        /// <param name="source">The path of the source folder</param>
        /// <param name="width">The width of the thumbnail</param>
        /// <param name="height">The height of the thumbnail</param>
        /// <param name="keepRatio">true if the user wants to keep the ratio of the image, otherwise false</param>
        /// <returns>A dictionary with the image size for every image. The key is the name of the image</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        public static Dictionary<string, ImageSize> CreateThumbnails(string source, int width, int height, bool keepRatio)
        {
            // Step 1: Check the parameters
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException("The specified source doesn't exist.");

            var result = new Dictionary<string, ImageSize>();

            // Step 2: Create Thumbnail folder
            if (!CreateThumbnailFolder(source))
            {
                OnNewInfo?.Invoke(GlobalHelper.InfoType.Error, "Can't create thumbnail folder.");
                return result;
            }

            // Step 3: Load the images
            var imagesFiles = GlobalHelper.GetImageFiles(source);

            // Step 4: Itterate through the image list and create for every image a thumbnail
            var count = 1;

            foreach (var image in imagesFiles)
            {
                OnProgress?.Invoke(GlobalHelper.CalculateCurrentProgress(count++, imagesFiles.Count), 100);
                OnNewInfo?.Invoke(GlobalHelper.InfoType.Info, $"Create thumbnail {count} of {imagesFiles.Count}");

                var imageSize = new ImageSize(width, height);
                if (keepRatio || height == 0 && width != 0 || height != 0 && width == 0)
                    imageSize = CalculateImageSize(image.File, width, height);

                var newImage = GlobalHelper.ResizeImage(image.File, imageSize.Width, imageSize.Height);

                newImage.Save(Path.Combine(source, ThumbnailFolderName, image.File.Name));

                result.Add(image.File.Name, imageSize);
            }

            return result;
        }
        /// <summary>
        /// Creates the thumbnail folder in 
        /// </summary>
        /// <param name="source">The path of the source folder</param>
        /// <returns></returns>
        private static bool CreateThumbnailFolder(string source)
        {
            var thumbnailFolderPath = Path.Combine(source, ThumbnailFolderName);

            if (Directory.Exists(thumbnailFolderPath))
                return true;

            Directory.CreateDirectory(thumbnailFolderPath);
            return true;
        }
        /// <summary>
        /// Calculates the size of the thumbnail image if the user wants to keep the ratio of the image
        /// </summary>
        /// <param name="imageFile">The <see cref="FileInfo"/> object of the image</param>
        /// <param name="newWidth">The new width</param>
        /// <param name="newHeight">The new height</param>
        /// <returns>The new image size (<see cref="ImageSize"/>)</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        private static ImageSize CalculateImageSize(FileInfo imageFile, int newWidth, int newHeight)
        {
            if (imageFile == null)
                throw new ArgumentNullException(nameof(imageFile));

            if (newWidth == 0 && newHeight == 0)
                throw new ArgumentException("The new height and width are both 0.");

            var image = Image.FromFile(imageFile.FullName);

            // Formula: 
            // - with new width: (original height / original width) x new width = new height
            // - with new height: (original width / original height) x new height = new width
            var tmpHeight = (double)newHeight;
            var tmpWidth = (double)newWidth;

            if (newWidth != 0 && newHeight == 0)
            {
                tmpHeight = (double)image.Height / image.Width * newWidth;
            }
            else if (newWidth == 0 && newHeight != 0)
            {
                tmpWidth = (double)image.Width / image.Height * newHeight;
            }

            var result = new ImageSize((int)tmpWidth, (int)tmpHeight);

            return result;
        }
    }
}
