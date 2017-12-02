using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HtmlPictureTableCreator.DataObjects;
using HtmlPictureTableCreator.ViewModel;

namespace HtmlPictureTableCreator.View
{
    /// <summary>
    /// Interaction logic for CustomFooterWindow.xaml
    /// </summary>
    public partial class CustomFooterWindow : Window
    {
        /// <summary>
        /// Gets the image list
        /// </summary>
        public List<ImageModel> ImageList => DataContext is CustomFooterWindowViewModel viewModel
            ? viewModel.ImageList.ToList()
            : new List<ImageModel>();

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public CustomFooterWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="imageList">The list of images</param>
        public CustomFooterWindow(List<ImageModel> imageList)
        {
            InitializeComponent();
            DataContext = new CustomFooterWindowViewModel(imageList);
        }

        /// <summary>
        /// Occurs when the user hits the ok button
        /// </summary>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Occurs when the user hits the cancel button
        /// </summary>
        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
