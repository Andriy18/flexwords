using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.Helpers;

namespace FlexWords.Dialog.Handlers
{
    public sealed class LetterControl : TextBlock, ICalculatedWidth
    {
        public int LetterIndex { get; set; }
        public double CalculatedWidth { get; set; }

        public new Thickness Margin
        {
            get => base.Margin;
            set
            {
                if (base.Margin == value) return;

                base.Margin = value;
                RecalculateWidth();
            }
        }

        public ThemeSet Readspace
        {
            set
            {
                FontSize = Options.FontSize;
                FontWeight = FontWeightHelper.GetFontWeight(Options.FontWeight);
                FontFamily = new FontFamily(FontFamilyHelper.FontFamilyNames[Options.FontFamily]);
                
                if (LetterIndex > 0)
                {
                    Margin = new Thickness(Options.Kerning, 0, 0, 0);
                }
            }
        }

        public LetterControl()
        {
            HorizontalAlignment = HorizontalAlignment.Center;
        }

        public void RecalculateWidth()
        {
            CalculatedWidth = MeasureCore(ControlConstants.MeasureSize).Width;
        }

        public void SetForeground(ThemeSet workspace, bool selected = false)
        {
            if (selected) Foreground = workspace.SelectedForeground.ToBrush();
            else Foreground = workspace.Foreground.ToBrush();
        }

        public void Reset()
        {
            LetterIndex = 0;
            CalculatedWidth = 0;
            Margin = ControlConstants.BaseMargin;
        }

        public override string ToString()
        {
            return string.Format("{0} '{1}' {2}px ind:{3}",
                nameof(LetterControl),
                Text,
                CalculatedWidth,
                LetterIndex);
        }
    }
}
