using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using HtmlPictureTableCreator.Business;
using HtmlPictureTableCreator.DataObjects;
using HtmlPictureTableCreator.Global;
using HtmlPictureTableCreator.View;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfUtility.Services;

namespace HtmlPictureTableCreator.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        #region Properties for the view
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
        private bool _createThumbnails = true;
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
            set
            {
                SetField(ref _imageFooter, value);
                OnPropertyChanged(nameof(IsCustomFooter));
            }
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
        /// Contains the image ratio list
        /// </summary>
        public List<ComboBoxItem> RatioList { get; } = new List<ComboBoxItem>();

        /// <summary>
        /// Contains the selected ratio entry
        /// </summary>
        private ComboBoxItem _selectedRatio =
            new ComboBoxItem(GlobalHelper.GetEnumDescription(GlobalHelper.ImageRatio.Custom),
                (int) GlobalHelper.ImageRatio.Custom);
        /// <summary>
        /// Gets or sets the selected ratio entry
        /// </summary>
        public ComboBoxItem SelectedRatio
        {
            get => _selectedRatio;
            set
            {
                SetField(ref _selectedRatio, value);
                KeepRatioEnabled = value.Value == (int) GlobalHelper.ImageRatio.Custom;
            }
        }

        /// <summary>
        /// Contains the value which indicates if there is a "fix" ratio selected
        /// </summary>
        private bool _keppRatioEnabled;
        /// <summary>
        /// Gets or sets the fix ratio value
        /// </summary>
        public bool KeepRatioEnabled
        {
            get => _keppRatioEnabled;
            set => SetField(ref _keppRatioEnabled, value);
        }

        /// <summary>
        /// Contains the is running value
        /// </summary>
        private bool _isRunning;
        /// <summary>
        /// Gets or sets the value which indicates if the converting is running
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            set => SetField(ref _isRunning, value);
        }

        /// <summary>
        /// Contains the max value for the progress bar
        /// </summary>
        private double _maxValue = 1;
        /// <summary>
        /// Gets or sets the max value for the progress bar
        /// </summary>
        public double MaxValue
        {
            get => _maxValue;
            set => SetField(ref _maxValue, value);
        }

        /// <summary>
        /// Contains the current value for the progress bar
        /// </summary>
        private double _currentValue;
        /// <summary>
        /// Gets or sets the current value for the progress bar
        /// </summary>
        public double CurrentValue
        {
            get => _currentValue;
            set => SetField(ref _currentValue, value);
        }

        /// <summary>
        /// Contains the text for the progress bar
        /// </summary>
        private string _percentage = "";
        /// <summary>
        /// Gets or sets the percentage text for the progress bar
        /// </summary>
        public string Percentage
        {
            get => _percentage;
            set => SetField(ref _percentage, value);
        }
        
        /// <summary>
        /// Gets the custom footer flag
        /// </summary>
        public bool IsCustomFooter => (ImageFooter?.Value ?? 0) == 4;
        #endregion
        /// <summary>
        /// Contains the image list
        /// </summary>
        private List<ImageModel> _imageList = new List<ImageModel>();

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
        /// The command for the custom footer
        /// </summary>
        public ICommand CustomFooterCommand => new DelegateCommand(CustomFooter);

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

            foreach (var value in Enum.GetValues(typeof(GlobalHelper.ImageRatio)))
            {
                RatioList.Add(
                    new ComboBoxItem(GlobalHelper.GetEnumDescription((GlobalHelper.ImageRatio) value), (int) value));
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

            IsRunning = true;
            HtmlCreator.OnInfo += Helper_InfoEvent;
            HtmlCreator.OnProgress += HtmlCreator_OnProgress;
            Task.Factory.StartNew(() =>
            {
                var task = Task.Factory.StartNew(() => HtmlCreator.CreateHtmlTable(Source, CreateThumbnails,
                    ThumbnailHeight,
                    ThumbnailWidth, KeepRatioEnabled && KeepRatio, HeaderText, BlankTarget, ColumnCount,
                    (HtmlCreator.FooterType) ImageFooter.Value, CreateArchive, ArchiveName, OpenPage));

                task.Wait();
            }).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    InfoText += $"\r\n> Error | An error has occured. Message: {t.Exception.Message}";
                }
                HtmlCreator.OnInfo -= Helper_InfoEvent;
                HtmlCreator.OnProgress -= HtmlCreator_OnProgress;
                IsRunning = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        /// <summary>
        /// Occurs when the progress event was raised
        /// </summary>
        /// <param name="value">The current value</param>
        /// <param name="maxValue">The max value</param>
        private void HtmlCreator_OnProgress(double value, double maxValue)
        {
            MaxValue = maxValue;
            CurrentValue = value;
            Percentage = $"{value:N2}%";
        }
        /// <summary>
        /// Occurs when an info message was fired
        /// </summary>
        /// <param name="infoType">The type</param>
        /// <param name="message">The message</param>
        private void Helper_InfoEvent(GlobalHelper.InfoType infoType, string message)
        {
            WriteInfo(infoType, message);
        }
        /// <summary>
        /// Writes an info message
        /// </summary>
        /// <param name="infoType">The info type</param>
        /// <param name="message">The message</param>
        private void WriteInfo(GlobalHelper.InfoType infoType, string message)
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
                LoadImages();
            }
        }
        /// <summary>
        /// Calculate the opponent ratio value
        /// </summary>
        /// <param name="height">true to calculate the height, otherwise false</param>
        public void CalculateRatio(bool height)
        {
            var ratio = (GlobalHelper.ImageRatio) SelectedRatio.Value;
            

            double result;
            var widthValue = 4;
            var heightValue = 3;
            switch (ratio)
            {
                case GlobalHelper.ImageRatio.FourToThree:
                    widthValue = 4;
                    heightValue = 3;
                    break;
                case GlobalHelper.ImageRatio.SixteenToNine:
                    widthValue = 16;
                    heightValue = 9;
                    break;
                case GlobalHelper.ImageRatio.SixteenToTen:
                    widthValue = 16;
                    heightValue = 10;
                    break;
            }

            switch (ratio)
            {
                case GlobalHelper.ImageRatio.Custom:
                    result = height ? ThumbnailHeight : ThumbnailWidth;
                    break;
                default:
                    result = height
                        ? ThumbnailWidth / widthValue * heightValue
                        : ThumbnailHeight / heightValue * widthValue;
                    break;
            }

            if (height)
            {
                _thumbnailHeight = (int) result;
                OnPropertyChanged(nameof(ThumbnailHeight));
            }
            else
            {
                _thumbnailWidth = (int) result;
                OnPropertyChanged(nameof(ThumbnailWidth));
            }

        }

        /// <summary>
        /// Loads the images
        /// </summary>
        private void LoadImages()
        {
            if (!string.IsNullOrEmpty(Source))
            {
                _imageList = GlobalHelper.GetImageFiles(Source);
                WriteInfo(GlobalHelper.InfoType.Info, $"{_imageList.Count} images found.");
            }
        }

        /// <summary>
        /// Opens a window where the user can create a custom footer
        /// </summary>
        private void CustomFooter()
        {
            var dialog = new CustomFooterWindow(_imageList);

            if (dialog.ShowDialog() == true)
            {
                _imageList = dialog.ImageList;
            }
        }
    }
}
