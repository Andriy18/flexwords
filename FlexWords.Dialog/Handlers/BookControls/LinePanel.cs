using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlexWords.Dialog.Handlers
{
    public sealed class LinePanel : StackPanel, ICalculatedWidth
    {
        public int LineIndex { get; set; }

        public ThemeSet Readspace
        {
            set
            {
                if (LineIndex > 0)
                {
                    Margin = new Thickness(0, Options.LineSpacing, 0, 0);
                }
            }
        }

        public double CalculatedWidth
        {
            get => Children.OfType<ICalculatedWidth>().Sum(i => i.CalculatedWidth);
        }

        public int KerningCount
        {
            get => Items.Sum(i => i.KerningCount);
        }

        public IEnumerable<GroupPanel> Items
        {
            get => Children.Cast<GroupPanel>();
        }

        public LinePanel()
        {
            Orientation = Orientation.Horizontal;

            if (ControlConstants.value)
                Background = Brushes.DarkGreen;
        }

        public void Reset()
        {
            LineIndex = 0;
            Margin = ControlConstants.BaseMargin;
        }

        public void SetAdditionalKerning(double additionalKerning)
        {
            foreach (GroupPanel groupPanel in Items)
            {
                for (int j = 0; j < groupPanel.Children.Count; j++)
                {
                    WordPanel wordPanel = (WordPanel)groupPanel.Children[j];

                    if (wordPanel.WordIndex > 0)
                    {
                        wordPanel.Margin = new Thickness(wordPanel.Margin.Left + additionalKerning, 0, 0, 0);
                    }

                    for (int k = 0; k < wordPanel.Children.Count; k++)
                    {
                        LetterControl control = (LetterControl)wordPanel.Children[k];

                        if (control.LetterIndex > 0)
                        {
                            control.Margin = new Thickness(control.Margin.Left + additionalKerning, 0, 0, 0);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}px li:{2}",
                nameof(LinePanel),
                CalculatedWidth,
                LineIndex);
        }
    }
}
