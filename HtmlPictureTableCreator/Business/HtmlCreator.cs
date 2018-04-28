using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using HtmlPictureTableCreator.DataObjects;
using HtmlPictureTableCreator.Global;
using HtmlPictureTableCreator.HtmlRessources;
using Ionic.Zip;

namespace HtmlPictureTableCreator.Business
{
    public static class HtmlCreator
    {

        /// <summary>
        /// Occurs when an info was raised
        /// </summary>
        public static event GlobalHelper.InfoEvent OnInfo;
        /// <summary>
        /// Occurs when the progress goes on
        /// </summary>
        public static event GlobalHelper.ProgressEvent OnProgress;

        /// <summary>
        /// Creates the html table
        /// </summary>
        /// <param name="imageFiles">The image files</param>
        /// <param name="settings">The settings</param>
        public static void CreateHtmlTable(HtmlPageSettingsModel settings, List<ImageModel> imageFiles)
        {
            try
            {
                var archiveName = CreateArchiveName(settings.ArchiveName ?? "");

                var imageSizeList = new Dictionary<string, ImageSize>();
                if (settings.CreateThumbnails)
                {
                    ThumbnailManager.OnNewInfo += ThumbnailManagerOnNewInfo;
                    ThumbnailManager.OnProgress += ThumbnailManager_OnProgress;
                    imageSizeList = ThumbnailManager.CreateThumbnails(imageFiles, settings.Source, settings.Width, settings.Height, settings.KeepRatio);
                    ThumbnailManager.OnProgress -= ThumbnailManager_OnProgress;
                    ThumbnailManager.OnNewInfo -= ThumbnailManagerOnNewInfo;
                }

                var htmlTable = new StringBuilder("");
                var target = settings.BlankTarget ? "target=\"_blank\"" : "";

                var count = 1;
                var totalCount = 1;

                foreach (var image in imageFiles)
                {
                    OnInfo?.Invoke(GlobalHelper.InfoType.Info, $"Create image entry {totalCount} of {imageFiles.Count}");
                    OnProgress?.Invoke(GlobalHelper.CalculateCurrentProgress(totalCount, imageFiles.Count), 100);
                    if (count == 1)
                        htmlTable.AppendLine("<tr>");


                    if (!imageSizeList.TryGetValue(image.File.Name, out var imgSize))
                    {
                        imgSize = ImageSize.CreateDefault();
                    }

                    // Create the tag for the size
                    var imgSizeHtml = $"width=\"{imgSize.Width}\" height=\"{imgSize.Height}\"";

                    // Create the image tag
                    var thumbnail = settings.CreateThumbnails
                        ? $"<img src=\"thumbnails/{image.File.Name}\" {imgSizeHtml} alt=\"{image.File.Name}\" title=\"{image.File.Name}\">"
                        : $"<img src=\"{image.File.Name}\" {imgSizeHtml} alt=\"{image.File.Name}\" title=\"{image.File.Name}\">";

                    htmlTable.AppendLine(
                        $"<td><a href=\"{image.File.Name}\" {target}>{thumbnail}</a>{CreateImageFooter(image, settings.FooterType, totalCount, imageFiles.Count)}</td>");

                    count++;
                    totalCount++;

                    if (count <= settings.ColumnCount)
                        continue;
                    count = 1;
                    htmlTable.AppendLine("</tr>");
                }

                var archiveHtml = "";
                if (settings.CreateArchive)
                {
                    if (CreateArchive(settings.Source, archiveName))
                        archiveHtml = $"You can download all pictures here: <a href=\"{CreateArchiveName(archiveName)}\">{CreateArchiveName(archiveName)}</a><br /><br />";
                }

                OnInfo?.Invoke(GlobalHelper.InfoType.Info, "Write data into file.");
                var indexPath = Path.Combine(settings.Source, "index.html");
                File.WriteAllText(indexPath, CreateFinaleHtml(settings.Header, htmlTable.ToString(), archiveHtml, settings.Background.ToHtmlColor(), settings.Foreground.ToHtmlColor()));

                if (File.Exists(indexPath))
                {
                    OnInfo?.Invoke(GlobalHelper.InfoType.Info, "File created.");
                    if (settings.OpenPage)
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
        /// Occurs when the thumbnail manager raises the onprogress event
        /// </summary>
        /// <param name="value">The current value</param>
        /// <param name="maxValue">The max value</param>
        private static void ThumbnailManager_OnProgress(double value, double maxValue)
        {
            OnProgress?.Invoke(value, maxValue);
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
        private static string CreateImageFooter(ImageModel imageFile, GlobalHelper.FooterType type, int count, int totalCount )
        {
            if (imageFile == null)
                throw new ArgumentNullException(nameof(imageFile));

            var stringBuilder = new StringBuilder("<br/>");

            switch (type)
            {
                case GlobalHelper.FooterType.ImageName:
                    stringBuilder.Append(imageFile.File.Name);
                    break;
                case GlobalHelper.FooterType.Numbering:
                    stringBuilder.Append($"{count} of {totalCount}");
                    break;
                case GlobalHelper.FooterType.FileDetails:
                    var image = Image.FromFile(imageFile.File.FullName);
                    var detailTable = new StringBuilder("<table border='0' cellspacing='0' cellpadding='3'>");
                    detailTable.AppendLine($"<tr><td align='right'>File:</td><td>{imageFile.File.Name}</td></tr>");
                    detailTable.AppendLine($"<tr><td align='right'>Date:</td><td>{imageFile.File.CreationTime:g}</td></tr>");
                    detailTable.AppendLine($"<tr><td align='right'>Size:</td><td>{image.Width}x{image.Height}</td></tr>");
                    detailTable.AppendLine(
                        $"<tr><td align='right'>File size:</td><td>{(double) imageFile.File.Length / 1024 / 1024:N2} MB</td></tr>");
                    detailTable.AppendLine("</table>");
                    stringBuilder.Append(detailTable);
                    break;
                case GlobalHelper.FooterType.Custom:
                    stringBuilder.AppendLine($"{imageFile.Footer}");
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
        /// <param name="background">The hex code of the background color</param>
        /// <param name="foreground">The hex code of the foreground color</param>
        /// <returns>The final html file</returns>
        private static string CreateFinaleHtml(string title, string table, string archive, string background, string foreground)
        {
            var page = HtmlResources.HtmlText;
            page = page.Replace("[TITLE]", title);
            page = page.Replace("[TABLE]", table);
            page = page.Replace("[BACKGROUND]", background);
            page = page.Replace("[FOREGROUND]", foreground);
            page = page.Replace("[COPYRIGHT]",
                $"&copy; {DateTime.Now:g} - Created with <i><a href='https://github.com/InvaderZim85/HtmlPictureTableCreator' target='_blank'>HTML - Picture table creator</a></i>");
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
                // Calculates the percent
                string CalculatePercent(int step, int max)
                {
                    return $"{100d / max * step:N2}";
                }

                var archivePath = Path.Combine(source, archiveName);
                if (File.Exists(archivePath))
                    File.Delete(archivePath);

                var count = 1;
                using (var zipFile = new ZipFile())
                {
                    var files = GlobalHelper.GetImageFiles(source);

                    foreach (var file in files)
                    {
                        OnInfo?.Invoke(GlobalHelper.InfoType.Info, $"Create archive ({CalculatePercent(count++, files.Count)}%)");
                        OnProgress?.Invoke(GlobalHelper.CalculateCurrentProgress(count, files.Count), 100);
                        zipFile.AddFile(file.File.FullName);
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
        private static void ThumbnailManagerOnNewInfo(GlobalHelper.InfoType infoType, string message)
        {
            OnInfo?.Invoke(infoType, message);
        }
    }
}
