using System.Windows;
using System.Windows.Controls;
using HtmlPictureTableCreator.ViewModel;

namespace HtmlPictureTableCreator.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Occurs when the text of the textbox was changed
        /// </summary>
        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.ScrollToEnd();
            }
        }
        /// <summary>
        /// Occurs when the window is loading
        /// </summary>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.InitViewModel();
            }
        }
        /// <summary>
        /// Occurs when the width was changed
        /// </summary>
        private void ThumbnailWidth_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.CalculateRatio(true);
            }
        }
        /// <summary>
        /// Occurs when the height was changed
        /// </summary>
        private void ThumbnailHeight_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.CalculateRatio(false);
            }
        }
    }
}
