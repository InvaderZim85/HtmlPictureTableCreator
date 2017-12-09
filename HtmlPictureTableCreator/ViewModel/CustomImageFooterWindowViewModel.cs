using System;
using System.Collections.Generic;
using System.Drawing;
using HtmlPictureTableCreator.DataObjects;
using WpfUtility.Services;

namespace HtmlPictureTableCreator.ViewModel
{
    public class CustomImageFooterWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Contains the current page
        /// </summary>
        private int _currentPage = 1;
        /// <summary>
        /// Contains the max pages
        /// </summary>
        private readonly int _maxPages = 0;

        /// <summary>
        /// The different movement types
        /// </summary>
        private enum MovementTypes
        {
            /// <summary>
            /// Show the first page
            /// </summary>
            First,
            /// <summary>
            /// Show the previous page
            /// </summary>
            Previous,
            /// <summary>
            /// Show the next page
            /// </summary>
            Next,
            /// <summary>
            /// Show the last page
            /// </summary>
            Last
        }

        /// <summary>
        /// Contains the original image list
        /// </summary>
        private readonly List<ImageModel> _originalList;
        /// <summary>
        /// Gets the list with the imagesx
        /// </summary>
        public List<ImageModel> ImageList => _originalList;

        /// <summary>
        /// Contains the current image
        /// </summary>
        private ImageModel _currentImage;
        /// <summary>
        /// Gets or sets the current image
        /// </summary>
        public ImageModel CurrentImage
        {
            get => _currentImage;
            set
            {
                SetField(ref _currentImage, value);
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Size));
                OnPropertyChanged(nameof(Extension));
                OnPropertyChanged(nameof(Date));
                OnPropertyChanged(nameof(Dimension));
                OnPropertyChanged(nameof(Footer));
            }
        }

        /// <summary>
        /// Gets or sets the footer of the image
        /// </summary>
        public string Footer
        {
            get => _currentImage?.Footer ?? "";
            set
            {
                if (_currentImage != null && _currentImage.Footer != value)
                {
                    _currentImage.Footer = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Contains the info for the user
        /// </summary>
        private string _page;
        /// <summary>
        /// Gets or sets the page info for the user
        /// </summary>
        public string Page
        {
            get => _page;
            set => SetField(ref _page, value);
        }

        /// <summary>
        /// Gets the name of the file
        /// </summary>
        public string Name => _currentImage?.File.Name ?? "";
        /// <summary>
        /// Gets the extension of the file
        /// </summary>
        public string Extension => _currentImage?.File.Extension.Replace(".", "") ?? "";
        /// <summary>
        /// Gets the file size
        /// </summary>
        public string Size => $"{_currentImage?.File.Length / (double)1024 / 1024:N2} MB";
        /// <summary>
        /// Gets the date of the image
        /// </summary>
        public string Date => _currentImage?.File.CreationTime.ToString("g");
        /// <summary>
        /// Gets the dimension of the image
        /// </summary>
        public string Dimension
        {
            get
            {
                var result = "";
                if (_currentImage != null)
                {
                    var image = Image.FromFile(_currentImage.File.FullName);
                    result = $"{image.Width}x{image.Height}";
                }
                return result;
            }
        }

        /// <summary>
        /// Creates a new, empty instance of the view model
        /// </summary>
        public CustomImageFooterWindowViewModel() { }

        /// <summary>
        /// Creates a new instance of the view model
        /// </summary>
        /// <param name="imageList">The image list</param>
        public CustomImageFooterWindowViewModel(List<ImageModel> imageList)
        {
            _originalList = imageList;

            _maxPages = imageList.Count;

            Movement(MovementTypes.First);
        }

        /// <summary>
        /// Shows the first page
        /// </summary>
        public DelegateCommand FirstCommand => new DelegateCommand(() => Movement(MovementTypes.First));
        /// <summary>
        /// Shows the previous page
        /// </summary>
        public DelegateCommand PreviousCommand => new DelegateCommand(() => Movement(MovementTypes.Previous));
        /// <summary>
        /// Shows the next page
        /// </summary>
        public DelegateCommand NextCommand => new DelegateCommand(() => Movement(MovementTypes.Next));
        /// <summary>
        /// Shows the last page
        /// </summary>
        public DelegateCommand LastCommand => new DelegateCommand(() => Movement(MovementTypes.Last));
        /// <summary>
        /// Resets the command
        /// </summary>
        public DelegateCommand ResetCommand => new DelegateCommand(() => Footer = "");

        /// <summary>
        /// Moves the pages
        /// </summary>
        /// <param name="movement">The movement type</param>
        private void Movement(MovementTypes movement)
        {
            switch (movement)
            {
                case MovementTypes.First:
                    if (_currentPage == 0)
                        return;

                    _currentPage = 0;
                    break;
                case MovementTypes.Previous:
                    if (_currentPage == 0)
                        return;

                    if (_currentPage >= 1)
                        _currentPage--;
                    break;
                case MovementTypes.Next:
                    if (_currentPage == _maxPages - 1)
                        return;

                    if (_currentPage < _maxPages - 1)
                        _currentPage++;
                    break;
                case MovementTypes.Last:
                    if (_currentPage == _maxPages - 1)
                        return;

                    _currentPage = _maxPages - 1;
                    break;
            }

            CurrentImage = _originalList[_currentPage];

            Page = $"Image {_currentPage + 1} of {_maxPages} - {CurrentImage?.File.Name ?? ""}";
            GC.Collect();
        }
    }
}
