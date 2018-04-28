using System.Windows.Media;
using HtmlPictureTableCreator.Global;

namespace HtmlPictureTableCreator.DataObjects
{
    public class HtmlPageSettingsModel
    {
        /// <summary>
        /// Gets or sets the source path
        /// </summary>
        public string Source { get; set; } = "";
        /// <summary>
        /// Gets or sets the value which indicates if the user wants to create thumbnails
        /// </summary>
        public bool CreateThumbnails { get; set; }
        /// <summary>
        /// Gets or sets the value which indicates if the thumbnail should keep the ratio of the image
        /// </summary>
        public bool KeepRatio { get; set; }

        /// <summary>
        /// Gets or sets the image ratio
        /// </summary>
        public GlobalHelper.ImageRatio ImageRatio { get; set; } = GlobalHelper.ImageRatio.Custom;
        /// <summary>
        /// Gets or sets the width of the image
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Gets or sets the height of the image
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Gets or sets the value which indicates if the user wants to use custom colors
        /// </summary>
        public bool CustomColors { get; set; }

        /// <summary>
        /// Gets or sets the background color
        /// </summary>
        public Color Background { get; set; } = Color.FromRgb(0, 0, 0);

        /// <summary>
        /// Gets or sets the foreground color
        /// </summary>
        public Color Foreground { get; set; } = Color.FromRgb(255, 255, 255);

        /// <summary>
        /// Gets or sets the column count
        /// </summary>
        public int ColumnCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets the header text
        /// </summary>
        public string Header { get; set; } = "";
        /// <summary>
        /// Gets or sets the value which indicates if the image link should be opened in a new tab
        /// </summary>
        public bool BlankTarget { get; set; }

        /// <summary>
        /// Gets or sets the footer type (<see cref="GlobalHelper.FooterType"/>)
        /// </summary>
        public GlobalHelper.FooterType FooterType { get; set; } = GlobalHelper.FooterType.Nothing;
        /// <summary>
        /// Gets or sets the value which indicates if the user wants to create a zip archive
        /// </summary>
        public bool CreateArchive { get; set; }

        /// <summary>
        /// Gets or sets the archive name
        /// </summary>
        public string ArchiveName { get; set; } = "";
        /// <summary>
        /// Gets or sets the value which indicates if the page should be opened at the end
        /// </summary>
        public bool OpenPage { get; set; }
    }
}
