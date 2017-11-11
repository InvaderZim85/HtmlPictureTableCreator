using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using HtmlPictureTableCreator.DataObjects;
using HtmlPictureTableCreator.HtmlRessources;
using Ionic.Zip;

namespace HtmlPictureTableCreator
{
    public class Helper
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
        /// Contains the different file types
        /// </summary>
        private readonly string[] _fileTypes = new[] {".jpeg", ".jpg", ".png", ".bmp"};

        /// <summary>
        /// The delegate for the info event
        /// </summary>
        /// <param name="infoType">The type of the message</param>
        /// <param name="message">The info message</param>
        public delegate void InfoMessage(InfoType infoType, string message);

        /// <summary>
        /// Occurs when an info was raised
        /// </summary>
        public event InfoMessage InfoEvent;
        /// <summary>
        /// Contains the image sizes for the converted pictures. The key is the name of the image
        /// </summary>
        private readonly Dictionary<string, ImageSize> _imageSizeList = new Dictionary<string, ImageSize>();


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
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
        /// Creates the html table
        /// </summary>
        /// <param name="source">The source path</param>
        /// <param name="createThumbnails">The value which indicates if the user want to use thumbnails</param>
        /// <param name="thumbHeight">The height of the thumbnail</param>
        /// <param name="thumbWidth">The width of the thumbnail</param>
        /// <param name="keepRatio">The value which indicates if the ratio should be keeped</param>
        /// <param name="headerText">The headertext</param>
        /// <param name="blankTarget">true to use a blank target</param>
        /// <param name="columnCount">The column count</param>
        /// <param name="imageFooter">The image footer id</param>
        /// <param name="createArchive">true if the user wants to create a archive</param>
        /// <param name="archiveName">The name of the archive</param>
        public void CreateHtmlTable(string source, bool createThumbnails, int thumbHeight, int thumbWidth,
            bool keepRatio, string headerText, bool blankTarget, int columnCount, int imageFooter, bool createArchive, string archiveName)
        {
            try
            {
                if (string.IsNullOrEmpty(source))
                {
                    InfoEvent?.Invoke(InfoType.Error, "The source path is null or empty.");
                    return;
                }

                if (createThumbnails)
                {
                    if (!CreateThumbnails(source, thumbHeight, thumbWidth, keepRatio))
                        createThumbnails = false;
                }

                var tableText = new StringBuilder("");
                var target = blankTarget ? "" : "target=\"_blank\"";

                var imageFiles = GetFiles(source);


                var count = 1;
                var totalCount = 1;

                foreach (var image in imageFiles)
                {
                    InfoEvent?.Invoke(InfoType.Info, $"Create image entry {totalCount} of {imageFiles.Count}");
                    if (count == 1)
                        tableText.AppendLine("<tr>");


                    if (!_imageSizeList.TryGetValue(image.Name, out var imgSize))
                    {
                        imgSize = ImageSize.CreateDefault();
                    }

                    var imgSizeHtml = $"width=\"{imgSize.Width}\" height=\"{imgSize.Height}\"";


                    var thumbnail = createThumbnails
                        ? $"<img src=\"thumbnails/{image.Name}\" {imgSizeHtml} alt=\"{image.Name}\" title=\"{image.Name}\">"
                        : $"<img src=\"{image.Name}\" {imgSizeHtml} alt=\"{image.Name}\" title=\"{image.Name}\">";

                    string imgFooter;
                    switch (imageFooter)
                    {
                        case 1:
                            imgFooter = $"<br/>Image: {image.Name}";
                            break;
                        case 2:
                            imgFooter = $"<br/>Image {totalCount}/{imageFiles.Count}";
                            break;
                        default:
                            imgFooter = "";
                            break;
                    }

                    tableText.AppendLine($"<td><a href=\"{image.Name}\" {target}>{thumbnail}</a>{imgFooter}</td>");

                    count++;
                    totalCount++;

                    if (count > columnCount)
                    {
                        count = 1;
                        tableText.AppendLine("</tr>");

                    }
                }

                var archiveHtml = "";
                if (createArchive)
                {
                    if (CreateArchive(source, archiveName))
                        archiveHtml = $"You can download all pictures here: <a href=\"{CreateArchiveName(archiveName)}\">{CreateArchiveName(archiveName)}</a><br /><br />";
                }

                var copyright = "Created with <i>HTML - Picture table creator</i>";
                var htmlText = HtmlResources.HtmlText;
                htmlText = htmlText.Replace("[TITLE]", headerText).Replace("[TABLE]", tableText.ToString())
                    .Replace("[COPYRIGHT]", copyright).Replace("[ARCHIVE]", archiveHtml);

                InfoEvent?.Invoke(InfoType.Info, "Write data into file.");
                var indexPath = Path.Combine(source, "index.html");
                File.WriteAllText(indexPath, htmlText);

                if (File.Exists(indexPath))
                    InfoEvent?.Invoke(InfoType.Info, "File created.");
                else
                    InfoEvent?.Invoke(InfoType.Error, "Can't create file.");
            }
            catch (Exception ex)
            {
                InfoEvent?.Invoke(InfoType.Error, $"An error has occured. Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates the thumbnails for the files which are available in the folder
        /// </summary>
        /// <param name="source">The source folder</param>
        /// <param name="height">The height of the thumbnail</param>
        /// <param name="width">The width of the thumbnail</param>
        /// <param name="keepRatio">true if the ratio should be keeped, otherwise false</param>
        private bool CreateThumbnails(string source, int height, int width, bool keepRatio)
        {
            // Step 1: Create the thumbnail folder
            if (!CreateThumbnailFolder(source))
                return false;

            // Step 1: Get all files in the source folder
            var imageFiles = GetFiles(source);

            foreach (var imageFile in imageFiles)
            {
                InfoEvent?.Invoke(InfoType.Info, $"Resizse image {imageFile.Name}");
                try
                {
                    var originImage = Image.FromFile(imageFile.FullName);
                    
                    var newSize = new ImageSize(400, 300);
                    if (keepRatio || height == 0 && width != 0 || height != 0 && width == 0)
                    {
                        newSize = CalculateSize(imageFile.Name, originImage, width, height);
                    }

                    var newImage = ResizeImage(originImage, newSize.Width, newSize.Height);

                    var newImagePath = Path.Combine(source, "thumbnails", imageFile.Name);

                    newImage.Save(newImagePath);
                }
                catch (Exception ex)
                {
                    InfoEvent?.Invoke(InfoType.Error, $"Can't create thumbnail for {imageFile.Name}. Skip thumbnail creation. Message: {ex.Message}");
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Creates the thumbnail folder
        /// </summary>
        /// <param name="source">The path of the source folder</param>
        /// <returns>true if successful, otherwise false</returns>
        private bool CreateThumbnailFolder(string source)
        {
            var path = Path.Combine(source, "thumbnails");

            if (Directory.Exists(path))
                return true;
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                InfoEvent?.Invoke(InfoType.Error, $"Can't create thumbnail folder. Skip thumbnail creation. Message: {ex.Message}");
                return false;
            }

        }
        /// <summary>
        /// Loads the image files
        /// </summary>
        /// <param name="path">The path of the folder</param>
        /// <returns>The image files</returns>
        private List<FileInfo> GetFiles(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var dirInfo = new DirectoryInfo(path);
            var tmpFiles = dirInfo.GetFiles();

            return tmpFiles.Where(w => _fileTypes.Contains(w.Extension.ToLower())).ToList();
        }

        /// <summary>
        /// Calculates the image size for the thumbnail
        /// </summary>
        /// <param name="imgName">The name of the image</param>
        /// <param name="originalImage">The original image</param>
        /// <param name="newWidth">The new width</param>
        /// <param name="newHeight">The new height</param>
        /// <returns>The calculated image size</returns>
        private ImageSize CalculateSize(string imgName, Image originalImage, int newWidth, int newHeight)
        {
            if (originalImage == null)
                return null;

            // Formula: 
            // - with new width: (original height / original width) x new width = new height
            // - with new height: (original width / original height) x new height = new width
            var tmpHeight = (double)newHeight;
            var tmpWidth = (double)newWidth;

            if (newWidth != 0 && newHeight == 0)
            {
                tmpHeight = (double)originalImage.Height / originalImage.Width * newWidth;
            }
            else if (newWidth == 0 && newHeight != 0)
            {
                tmpWidth = (double)originalImage.Width / originalImage.Height * newHeight;
            }

            var result = new ImageSize((int)tmpWidth, (int)tmpHeight);

            _imageSizeList.Add(imgName, result);

            return result;
        }
        /// <summary>
        /// Creates a download able archive
        /// </summary>
        /// <param name="source">The file source</param>
        /// <param name="archiveName">The archive name</param>
        /// <returns>true if successful, otherwise false</returns>
        private bool CreateArchive(string source, string archiveName)
        {
            try
            {
                var archivePath = CreateArchiveFilePath(source, archiveName);
                if (File.Exists(archivePath))
                    File.Delete(archivePath);

                using (var zipFile = new ZipFile())
                {
                    var files = GetFiles(source);

                    foreach (var file in files)
                    {
                        zipFile.AddFile(file.FullName);
                    }

                    zipFile.Save(archivePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                InfoEvent?.Invoke(InfoType.Error, $"An error has occured while creating the archive. Message: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Creates the path of the archive
        /// </summary>
        /// <param name="source">The path of the archive</param>
        /// <param name="archiveName">The archive name</param>
        /// <returns>The path</returns>
        private string CreateArchiveFilePath(string source, string archiveName)
        {
            if (!archiveName.EndsWith(".zip"))
                archiveName += ".zip";

            return Path.Combine(source, CreateArchiveName(archiveName));
        }
        /// <summary>
        /// Creates the archive name
        /// </summary>
        /// <param name="archiveName">The entered archiv name</param>
        /// <returns>The new archive name</returns>
        private string CreateArchiveName(string archiveName)
        {
            if (!archiveName.EndsWith(".zip"))
                archiveName += ".zip";

            return archiveName.Replace(" ", "_");
        }
    }
}
