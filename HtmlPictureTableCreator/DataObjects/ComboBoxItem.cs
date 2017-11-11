namespace HtmlPictureTableCreator.DataObjects
{
    public class ComboBoxItem
    {
        public string Text { get; set; } = "";
        public int Value { get; set; }

        public ComboBoxItem() { }

        public ComboBoxItem(string text, int value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
