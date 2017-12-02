using System;
using System.Drawing;
using System.IO;

namespace HtmlPictureTableCreator.DataObjects
{
    public class ImageModel
    {
        /// <summary>
        /// Gets or sets the image file
        /// </summary>
        public FileInfo File { get; set; }

        /// <summary>
        /// Gets or sets the export flag
        /// </summary>
        public bool ExportImage { get; set; } = true;

        /// <summary>
        /// Gets or sets the footer of the entry
        /// </summary>
        public string Footer { get; set; } = "";
        /// <summary>
        /// Gets the image uri
        /// </summary>
        public Uri ImageUri => new Uri(File?.FullName ?? "");

        /// <summary>
        /// Creates a new empty instance of the model
        /// </summary>
        public ImageModel() { }
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="fileInfo">The file</param>
        public ImageModel(FileInfo fileInfo)
        {
            File = fileInfo;
        }
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="filepath">THe path of the image</param>
        public ImageModel(string filepath)
        {
            File = new FileInfo(filepath);
        }
    }
}
