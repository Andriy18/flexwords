using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlexWords.Dialog.Handlers
{
    public sealed class GroupPanel : StackPanel, ICalculatedWidth
    {
        public int Index { get; set; }
        public int GroupIndex { get; set; }

        public ThemeSet Readspace
        {
            set
            {
                if (Index is 0)
                {
                    Margin = new Thickness(Options.Indent, 0, 0, 0);
                    return;
                }

                if (GroupIndex > 0)
                {
                    Margin = new Thickness(Options.WordSpacing, 0, 0, 0);
                }
            }
        }

        public double CalculatedWidth
        {
            get => Items.Sum(i => i.CalculatedWidth) + Margin.Left;
        }

        public int KerningCount
        {
            get => Items.Sum(i => i.Children.Count) - 1;
        }

        public IEnumerable<WordPanel> Items
        {
            get => Children.Cast<WordPanel>();
        }

        public GroupPanel()
        {
            Orientation = Orientation.Horizontal;

            if (ControlConstants.value)
                Background = Brushes.DarkBlue;
        }

        public void Add(WordPanel panel, int index, int wordIndex, ThemeSet workspace)
        {
            panel.Index = index;
            panel.WordIndex = wordIndex;
            panel.Readspace = workspace;
            panel.SoftUpdate(workspace);

            Children.Add(panel);
        }

        public void Reset()
        {
            Index = 0;
            GroupIndex = 0;
            Margin = ControlConstants.BaseMargin;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}px ind:{2}:{3}",
                nameof(GroupPanel),
                CalculatedWidth,
                Index,
                GroupIndex);
        }
    }
}
