using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlPictureTableCreator.DataObjects;
using WpfUtility.Services;

namespace HtmlPictureTableCreator.ViewModel
{
    public class CustomFooterWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Contains the current page
        /// </summary>
        private int _currentPage = 0;
        /// <summary>
        /// Contains the max pages
        /// </summary>
        private readonly int _maxPages = 0;
        /// <summary>
        /// Contains the amount of entries per page
        /// </summary>
        private int _entriesPerPage = 5;

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
        private List<ImageModel> _originalList;
        /// <summary>
        /// Contains the image list
        /// </summary>
        private ObservableCollection<ImageModel> _imageList = new ObservableCollection<ImageModel>();
        /// <summary>
        /// Gets or sets the list with the images
        /// </summary>
        public ObservableCollection<ImageModel> ImageList
        {
            get => _imageList;
            set => SetField(ref _imageList, value);
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
        /// Creates a new, empty instance of the view model
        /// </summary>
        public CustomFooterWindowViewModel() { }

        /// <summary>
        /// Creates a new instance of the view model
        /// </summary>
        /// <param name="imageList">The image list</param>
        public CustomFooterWindowViewModel(List<ImageModel> imageList)
        {
            _originalList = imageList;

            _maxPages = (int) Math.Ceiling((double) imageList.Count / _entriesPerPage);

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
        /// Moves the pages
        /// </summary>
        /// <param name="movement">The movement type</param>
        private void Movement(MovementTypes movement)
        {
            switch (movement)
            {
                case MovementTypes.First:
                    if (_currentPage == 1)
                        return;

                    _currentPage = 1;
                    break;
                case MovementTypes.Previous:
                    if (_currentPage == 1)
                        return;

                    if (_currentPage > 1)
                        _currentPage--;
                    break;
                case MovementTypes.Next:
                    if (_currentPage == _maxPages)
                        return;

                    if (_currentPage < _maxPages)
                        _currentPage++;
                    break;
                case MovementTypes.Last:
                    if (_currentPage == _maxPages)
                        return;

                    _currentPage = _maxPages;
                    break;
            }

            var skipValue = (_currentPage - 1) * _entriesPerPage;

            ImageList = null;
            ImageList = new ObservableCollection<ImageModel>(_originalList.Skip(skipValue).Take(_entriesPerPage));
            Page = $"Page {_currentPage} of {_maxPages} ({_entriesPerPage} entries per page)";
        }
    }
}
