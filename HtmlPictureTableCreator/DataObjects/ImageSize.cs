﻿namespace HtmlPictureTableCreator.DataObjects
{
    public class ImageSize
    {
        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Creates a new empty instance
        /// </summary>
        public ImageSize() { }
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        public ImageSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static ImageSize CreateDefault()
        {
            return new ImageSize(400, 300);
        }
    }
}
