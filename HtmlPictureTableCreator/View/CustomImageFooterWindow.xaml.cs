using System.Collections.Generic;
using System.Windows;
using HtmlPictureTableCreator.DataObjects;
using HtmlPictureTableCreator.ViewModel;

namespace HtmlPictureTableCreator.View
{
    /// <summary>
    /// Interaction logic for CustomImageFooterWindow.xaml
    /// </summary>
    public partial class CustomImageFooterWindow : Window
    {
        /// <summary>
        /// Creates a new instance of the window
        /// </summary>
        public CustomImageFooterWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Creates a new instance of the window
        /// </summary>
        /// <param name="imageList">The list with the images</param>
        public CustomImageFooterWindow(List<ImageModel> imageList)
        {
            InitializeComponent();

            DataContext = new CustomImageFooterWindowViewModel(imageList);
        }
        /// <summary>
        /// Gets the image list
        /// </summary>
        public List<ImageModel> ImageList => DataContext is CustomImageFooterWindowViewModel viewModel
            ? viewModel.ImageList
            : new List<ImageModel>();
        /// <summary>
        /// Occurs when the user hits the close button
        /// </summary>
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
