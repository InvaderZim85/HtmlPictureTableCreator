using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using HtmlPictureTableCreator.DataObjects;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfUtility.Services;

namespace HtmlPictureTableCreator
{
    public class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Contains the source path
        /// </summary>
        private string _source;
        /// <summary>
        /// Gets or sets the source path
        /// </summary>
        public string Source
        {
            get => _source;
            set => SetField(ref _source, value);
        }
        /// <summary>
        /// Contains the value which indicates if the user wants to create thumbnails
        /// </summary>
        private bool _createThumbnails;
        /// <summary>
        /// Gets or sets the CreateThumbnails value
        /// </summary>
        public bool CreateThumbnails
        {
            get => _createThumbnails;
            set => SetField(ref _createThumbnails, value);
        }
        /// <summary>
        /// Contains the thumbnail width
        /// </summary>
        private int _thumbnailWidth;
        /// <summary>
        /// Gets or sets the thumbnail width
        /// </summary>
        public int ThumbnailWidth
        {
            get => _thumbnailWidth;
            set
            {
                if (KeepRatio && value != 0)
                {
                    ThumbnailHeight = 0;
                }
                SetField(ref _thumbnailWidth, value);
            }
        }

        /// <summary>
        /// Contains the thumbnail height
        /// </summary>
        private int _thumbnailHeight;
        /// <summary>
        /// Gets or sets the thumbnail height
        /// </summary>
        public int ThumbnailHeight
        {
            get => _thumbnailHeight;
            set
            {
                if (KeepRatio && value != 0)
                    ThumbnailWidth = 0;

                SetField(ref _thumbnailHeight, value);
            }
        }

        /// <summary>
        /// Contains the keep ratio value
        /// </summary>
        private bool _keepRatio;
        /// <summary>
        /// Gets or sets the keep ratio value
        /// </summary>
        public bool KeepRatio
        {
            get => _keepRatio;
            set => SetField(ref _keepRatio, value);
        }
        /// <summary>
        /// Contains the column count
        /// </summary>
        private int _columnCount = 3;
        /// <summary>
        /// Gets or sets the column count
        /// </summary>
        public int ColumnCount
        {
            get => _columnCount;
            set => SetField(ref _columnCount, value);
        }
        /// <summary>
        /// Contains the header text
        /// </summary>
        private string _headerText = "";
        /// <summary>
        /// Gets or sets the header text
        /// </summary>
        public string HeaderText
        {
            get => _headerText;
            set => SetField(ref _headerText, value);
        }
        /// <summary>
        /// Contains the blank target value
        /// </summary>
        private bool _blankTarget;
        /// <summary>
        /// Gets or sets the blank target value
        /// </summary>
        public bool BlankTarget
        {
            get => _blankTarget;
            set => SetField(ref _blankTarget, value);
        }

        /// <summary>
        /// Gets the image footer list
        /// </summary>
        public List<ComboBoxItem> ImageFooterList { get; } = new List<ComboBoxItem>();

        /// <summary>
        /// Contains the image footer value
        /// </summary>
        private ComboBoxItem _imageFooter = new ComboBoxItem("Nothing", 0);
        /// <summary>
        /// Gets or sets the image footer value
        /// </summary>
        public ComboBoxItem ImageFooter
        {
            get => _imageFooter;
            set => SetField(ref _imageFooter, value);
        }
        /// <summary>
        /// Contains the info text
        /// </summary>
        private string _infoText = "HTML - Picture table creator";
        /// <summary>
        /// Gets or sets the info text
        /// </summary>
        public string InfoText
        {
            get => _infoText;
            set => SetField(ref _infoText, value);
        }
        /// <summary>
        /// Contains the create archive value
        /// </summary>
        private bool _createArchive;
        /// <summary>
        /// Gets or sets the create archive value
        /// </summary>
        public bool CreateArchive
        {
            get => _createArchive;
            set => SetField(ref _createArchive, value);
        }
        /// <summary>
        /// Contains the name of the archive
        /// </summary>
        private string _archiveName;
        /// <summary>
        /// Gets or sets the name of the archive
        /// </summary>
        public string ArchiveName
        {
            get => _archiveName;
            set => SetField(ref _archiveName, value);
        }
        /// <summary>
        /// Contains the open page value
        /// </summary>
        private bool _openPage = true;
        /// <summary>
        /// Gets or sets the open page value
        /// </summary>
        public bool OpenPage
        {
            get => _openPage;
            set => SetField(ref _openPage, value);
        }


        /// <summary>
        /// The start command
        /// </summary>
        public ICommand StartCommand => new DelegateCommand(Start);
        /// <summary>
        /// The reset command
        /// </summary>
        public ICommand ResetCommand => new DelegateCommand(Reset);
        /// <summary>
        /// The browse command
        /// </summary>
        public ICommand BrowseCommand => new DelegateCommand(Browse);
        /// <summary>
        /// Inits the view model
        /// </summary>
        public void InitViewModel()
        {
            foreach (var value in Enum.GetValues(typeof(HtmlCreator.FooterType)))
            {
                ImageFooterList.Add(
                    new ComboBoxItem(GlobalHelper.GetEnumDescription((HtmlCreator.FooterType) value), (int) value));
            }   
        }

        /// <summary>
        /// Starts the creation
        /// </summary>
        private void Start()
        {
            if (string.IsNullOrEmpty(Source))
            {
                InfoText += "\r\n No path selected.";
                return;
            }

            InfoText = "HTML - Picture table creator";

            HtmlCreator.OnInfo += Helper_InfoEvent;
            Task.Factory.StartNew(() =>
            {
                var task = Task.Factory.StartNew(() => HtmlCreator.CreateHtmlTable(Source, CreateThumbnails,
                    ThumbnailHeight,
                    ThumbnailWidth, KeepRatio, HeaderText, BlankTarget, ColumnCount,
                    (HtmlCreator.FooterType) ImageFooter.Value, CreateArchive, ArchiveName, OpenPage));

                task.Wait();
            }).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    InfoText += $"\r\n> Error | An error has occured. Message: {t.Exception.Message}";
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        /// <summary>
        /// Occurs when an info message was fired
        /// </summary>
        /// <param name="infoType">The type</param>
        /// <param name="message">The message</param>
        private void Helper_InfoEvent(GlobalHelper.InfoType infoType, string message)
        {
            InfoText += $"\r\n> {infoType.ToString()} | {message}";
        }

        /// <summary>
        /// Resets the settings
        /// </summary>
        private void Reset()
        {
            Source = "";
            CreateThumbnails = false;
            ColumnCount = 3;
            HeaderText = "";
            BlankTarget = true;
            KeepRatio = false;
            ThumbnailWidth = 0;
            ThumbnailHeight = 0;
            CreateArchive = false;
            ArchiveName = "";
            OpenPage = true;
            InfoText = "HTML - Picture table creator";

        }
        /// <summary>
        /// Browse for a folder
        /// </summary>
        private void Browse()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Source = dialog.FileName;
            }
        }
    }
}
