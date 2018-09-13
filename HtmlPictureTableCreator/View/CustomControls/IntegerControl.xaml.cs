using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HtmlPictureTableCreator.View.CustomControls
{
    /// <summary>
    /// Interaction logic for IntegerControl.xaml
    /// </summary>
    public partial class IntegerControl : UserControl
    {
        public IntegerControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(int), typeof(IntegerControl), new PropertyMetadata(default(int)));

        public int Value
        {
            get => (int) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            nameof(MinValue), typeof(int), typeof(IntegerControl), new PropertyMetadata(default(int)));

        public int MinValue
        {
            get => (int) GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            nameof(MaxValue), typeof(int), typeof(IntegerControl), new PropertyMetadata(default(int)));

        public int MaxValue
        {
            get => (int) GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        private void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValid(TextBox.Text);
        }

        private bool IsValid(string value)
        {
            return int.TryParse(value, out var result) && result >= MinValue && result <= MaxValue;
        }
    }
}
