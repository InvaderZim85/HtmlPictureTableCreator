# HTML - Picture table creator

## TOC
- [Overview](#overview)
- [Usage](#usage)
- [For developers](#for-developers)
- [ToDos](#todos)

## Overview

With this tool, you can create easily a *small* and *leightweight* HTML-page for a list of images. I've created this tool because I was tired to type everything on my own or use *heavy* tools to create a simple html page which shows only some images. With this tool I can create a html page, upload it to my webspace and share the link of the page with my friends to show them my images.

You can download the tool here: [HTML - Picture table creator](http://www.de-boddels.de/images/HtmlPictureTableCreator.zip)

## Usage

The usage is really straigth forward:

![window](http://www.de-boddels.de/images/window.png)

1. Select your source (enter the path of the directory with the images or press the *Browse* button)
2. Chose the option *Create thumbnails* if you want create thumbnails (smaller images)
    - If you choose *Keep ratio*, you can set the with or the height and the ratio of the original image will be keeped
    - Or you can select a defined ratio (4:3, 16:9, 16:10) and enter a width or height. The other value will be calculated automatically
    - Or you can select *Custom* and set custom values for the width and the height.
3. Choose the column count
4. Set a header / title for the page
5. Choose *Blank target* if the image behind the thumbnail should be opened in a new tab
6. Select an image footer (the footer will be shown the thumbnail)
    - Nothing: Thumbnail without a footer
    - Image name: The name of the image
    - Numbering: Numbering of the images e.g. 1/10, 2/10 and so on
    - Image details: Shows details like the image name, the original size, the file size and the creation date
    - Custom: If you choose *Custom*, you can click on the button *Footer* to add a custom footer for every image (The button *Footer* is only enabled if you choose *Custom*).
7. Choose *Create archive* to create a zip-archive. The archive will be added as a link to the end of the page.
8. Insert a name for the archive (spaces will be replaced by a underscore: "My Archive" > "My_Archive")
9. Choose *Open page* if the tool should open the page in your default browser at the end.

And this is the result:

![result](http://www.de-boddels.de/images/result.png)

## For developers

The tool is created with .NET-Framework 4.7.

Following NuGet-Packages are used:
- [DotNetZip](https://github.com/haf/DotNetZip.Semverd) for the zip file creation
- [Extended.Wpf.Toolkit](https://github.com/xceedsoftware/wpftoolkit) for the numeric up down box
- [Microsoft.WindowsAPICodePack-Shell](http://code.msdn.microsoft.com/WindowsAPICodePack) for the *FolderBrowseDialog*
    - Microsoft.WindowsAPICodePack-Core
- [MunkiWinchester.WpfUtility](https://github.com/MunkiWinchester/WpfUtility) for the MVVN-Pattern
- [NLog](http://nlog-project.org/) for logging
    - NLog.Config
    - NLog.Schema

## ToDos

The tool is in a very early stadium and some *features* are missing :) The following things are on my todo list:
- **General**
    - Logging of exceptions into a file via NLog. At the moment the logging is really *rudimentary*
    - ~~Possibility to add custom footers~~
- **Design**
    - Color-Chooser for the background / foreground color of the page
    - Font-Dialog to choose a another font
