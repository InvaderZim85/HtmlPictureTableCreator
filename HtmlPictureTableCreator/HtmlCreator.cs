using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    public static class HtmlCreator
    {
        /// <summary>
        /// The different footer types
        /// </summary>
        public enum FooterType
        {
            /// <summary>
            /// Show nothing
            /// </summary>
            [Description("Nothing")]
            Nothing = 0,
            /// <summary>
            /// Show the image name
            /// </summary>
            [Description("Image name")]
            ImageName = 1,
            /// <summary>
            /// Shows the numbering
            /// </summary>
            [Description("Numbering")]
            Numbering = 2,
            /// <summary>
            /// Shows image details like file size, date, etc.
            /// </summary>
            [Description("Image details")]
            FileDetails = 3
        }

        /// <summary>
        /// Occurs when an info was raised
        /// </summary>
        public static event GlobalHelper.InfoEvent OnInfo;

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
        /// <param name="openPage">true when the pages should be opened at the end</param>
        public static void CreateHtmlTable(string source, bool createThumbnails, int thumbHeight, int thumbWidth,
            bool keepRatio, string headerText, bool blankTarget, int columnCount, FooterType imageFooter, bool createArchive, string archiveName, bool openPage)
        {
            try
            {
                archiveName = CreateArchiveName(archiveName ?? "");

                if (string.IsNullOrEmpty(source))
                {
                    OnInfo?.Invoke(GlobalHelper.InfoType.Error, "The source path is null or empty.");
                    return;
                }

                var imageSizeList = new Dictionary<string, ImageSize>();
                if (createThumbnails)
                {
                    ThumbnailManager.OnNewInfo += ThumbnailManagerOnOnNewInfo;
                    imageSizeList = ThumbnailManager.CreateThumbnails(source, thumbWidth, thumbHeight, keepRatio);
                }

                var htmlTable = new StringBuilder("");
                var target = blankTarget ? "target=\"_blank\"" : "";

                var imageFiles = GlobalHelper.GetImageFiles(source);


                var count = 1;
                var totalCount = 1;

                foreach (var image in imageFiles)
                {
                    OnInfo?.Invoke(GlobalHelper.InfoType.Info, $"Create image entry {totalCount} of {imageFiles.Count}");
                    if (count == 1)
                        htmlTable.AppendLine("<tr>");


                    if (!imageSizeList.TryGetValue(image.Name, out var imgSize))
                    {
                        imgSize = ImageSize.CreateDefault();
                    }

                    // Create the tag for the size
                    var imgSizeHtml = $"width=\"{imgSize.Width}\" height=\"{imgSize.Height}\"";

                    // Create the image tag
                    var thumbnail = createThumbnails
                        ? $"<img src=\"thumbnails/{image.Name}\" {imgSizeHtml} alt=\"{image.Name}\" title=\"{image.Name}\">"
                        : $"<img src=\"{image.Name}\" {imgSizeHtml} alt=\"{image.Name}\" title=\"{image.Name}\">";

                    htmlTable.AppendLine(
                        $"<td><a href=\"{image.Name}\" {target}>{thumbnail}</a>{CreateImageFooter(image, imageFooter, totalCount, imageFiles.Count)}</td>");

                    count++;
                    totalCount++;

                    if (count <= columnCount)
                        continue;
                    count = 1;
                    htmlTable.AppendLine("</tr>");
                }

                var archiveHtml = "";
                if (createArchive)
                {
                    if (CreateArchive(source, archiveName))
                        archiveHtml = $"You can download all pictures here: <a href=\"{CreateArchiveName(archiveName)}\">{CreateArchiveName(archiveName)}</a><br /><br />";
                }

                OnInfo?.Invoke(GlobalHelper.InfoType.Info, "Write data into file.");
                var indexPath = Path.Combine(source, "index.html");
                File.WriteAllText(indexPath, CreateFinaleHtml(headerText, htmlTable.ToString(), archiveHtml));

                if (File.Exists(indexPath))
                {
                    OnInfo?.Invoke(GlobalHelper.InfoType.Info, "File created.");
                    if (openPage)
                        Process.Start(indexPath);
                }
                else
                    OnInfo?.Invoke(GlobalHelper.InfoType.Error, "Can't create file.");
            }
            catch (Exception ex)
            {
                OnInfo?.Invoke(GlobalHelper.InfoType.Error, $"An error has occured. Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates the html footer for the image
        /// </summary>
        /// <param name="imageFile">The <see cref="FileInfo"/> object of the image</param>
        /// <param name="type">The footer type</param>
        /// <param name="count">The current count</param>
        /// <param name="totalCount">The total count</param>
        /// <returns>The html footer for the image</returns>
        /// <exception cref="ArgumentNullException"/>
        private static string CreateImageFooter(FileInfo imageFile, FooterType type, int count, int totalCount )
        {
            if (imageFile == null)
                throw new ArgumentNullException(nameof(imageFile));

            var stringBuilder = new StringBuilder("<br/>");

            switch (type)
            {
                case FooterType.ImageName:
                    stringBuilder.Append(imageFile.Name);
                    break;
                case FooterType.Numbering:
                    stringBuilder.Append($"{count} of {totalCount}");
                    break;
                case FooterType.FileDetails:
                    var image = Image.FromFile(imageFile.FullName);
                    var detailTable = new StringBuilder("<table border='0' cellspacing='0' cellpadding='1'>");
                    detailTable.AppendLine($"<tr><td>File:</td><td>{imageFile.Name}</td></tr>");
                    detailTable.AppendLine($"<tr><td>Date:</td><td>{imageFile.CreationTime:g}</td></tr>");
                    detailTable.AppendLine($"<tr><td>Size:</td><td>{image.Width}x{image.Height}</td></tr>");
                    detailTable.AppendLine(
                        $"<tr><td>Filesize:</td><td>{(double) imageFile.Length / 1024 / 1024:N2}MB</td></tr>");
                    detailTable.AppendLine("</table>");
                    stringBuilder.Append(detailTable);
                    break;
                default:
                    stringBuilder.Clear();
                    break;
            }

            return stringBuilder.ToString();
        }
        /// <summary>
        /// Creates the complete html file
        /// </summary>
        /// <param name="title">The title of the page</param>
        /// <param name="table">The table</param>
        /// <param name="archive">The archive string</param>
        /// <returns>The final html file</returns>
        private static string CreateFinaleHtml(string title, string table, string archive)
        {
            var page = HtmlResources.HtmlText;
            page = page.Replace("[TITLE]", title);
            page = page.Replace("[TABLE]", table);
            page = page.Replace("[COPYRIGHT]",
                $"&copy; {DateTime.Now.Year}. Created with <i>HTML - Picture table creator</i>");
            page = page.Replace("[ARCHIVE]", archive);

            return page;
        }

        /// <summary>
        /// Creates a download able archive
        /// </summary>
        /// <param name="source">The file source</param>
        /// <param name="archiveName">The archive name</param>
        /// <returns>true if successful, otherwise false</returns>
        private static bool CreateArchive(string source, string archiveName)
        {
            try
            {
                var archivePath = Path.Combine(source, archiveName);
                if (File.Exists(archivePath))
                    File.Delete(archivePath);

                using (var zipFile = new ZipFile())
                {
                    var files = GlobalHelper.GetImageFiles(source);

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
                OnInfo?.Invoke(GlobalHelper.InfoType.Error, $"An error has occured while creating the archive. Message: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Creates the archive name
        /// </summary>
        /// <param name="archiveName">The entered archiv name</param>
        /// <returns>The new archive name</returns>
        private static string CreateArchiveName(string archiveName)
        {
            if (!archiveName.EndsWith(".zip"))
                archiveName += ".zip";

            return archiveName.Replace(" ", "_");
        }
        /// <summary>
        /// Occurs when the info message of the thumbnail creator was raised
        /// </summary>
        /// <param name="infoType">The info type</param>
        /// <param name="message">The message</param>
        private static void ThumbnailManagerOnOnNewInfo(GlobalHelper.InfoType infoType, string message)
        {
            OnInfo?.Invoke(infoType, message);
        }
    }
}
