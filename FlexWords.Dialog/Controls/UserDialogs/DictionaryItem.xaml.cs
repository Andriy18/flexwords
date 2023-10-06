using FlexWords.Constants;
using FlexWords.Dialog.Handlers;
using System.Windows.Media;

namespace FlexWords.Dialog.Controls.Components
{
    public partial class DictionaryItem : UserControlHandler
    {
        public char Text
        {
            set
            {
                textContainer.Text = value.ToString();
                intContainer.Text = ((int)value).ToString();
                textContainer.Foreground = CheckDictionary(value) ? Brushes.ForestGreen : Brushes.OrangeRed;
            }
        }

        public int Count
        {
            set
            {
                countContainer.Text = value.ToString();
            }
        }

        public DictionaryItem()
        {
            InitializeComponent();
        }

        private static bool CheckDictionary(char symbol)
        {
            return Words.DictionaryCheck.Contains(symbol);
        }
    }
}
