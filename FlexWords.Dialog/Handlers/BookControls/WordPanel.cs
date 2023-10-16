using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using FlexWords.Extensions.Specific;
using FlexWords.Dialog.Controls;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.ViewModels;
using FlexWords.Dialog.Animation;

namespace FlexWords.Dialog.Handlers
{
    public sealed class WordPanel : StackPanel, ICalculatedWidth
    {
        private bool _selected = false;
        private string _letterForeground = string.Empty;
        private string _selectedLetterForeground = string.Empty;
        private double _hoverOpacity;

        public bool Flag { get; set; }
        public bool IsTextType { get; set; }
        public int Index { get; set; }
        public int WordIndex { get; set; }
        public string Text { get; set; } = string.Empty;
        public FlexWordsDialog? Dialog { get; set; }
        public ThemeSet Readspace
        {
            set
            {
                if (WordIndex > 0)
                {
                    Margin = new Thickness(Options.Kerning, 0, 0, 0);
                }
            }
        }
        public double CalculatedWidth
        {
            get => Items.Sum(i => i.CalculatedWidth) + Margin.Left;
        }
        public IEnumerable<LetterControl> Items
        {
            get => Children.Cast<LetterControl>();
        }

        public WordPanel()
        {
            Orientation = Orientation.Horizontal;

            if (ControlConstants.value) Background = Brushes.Purple;
            else Background = Brushes.Transparent;
        }

        public void Add(LetterControl control, char letter, int index, ThemeSet workspace)
        {
            control.Text = letter.ToString();
            control.LetterIndex = index;
            control.Readspace = workspace;
            control.SetForeground(workspace);
            control.RecalculateWidth();

            if (!Flag) control.Foreground.Opacity = workspace.InactiveTextTransparency;

            Children.Add(control);
        }

        public void Reset()
        {
            AnimHelper.Flickering(this, true);
            _selected = false;
            Flag = false;
            IsTextType = false;
            Index = 0;
            WordIndex = 0;
            Text = string.Empty;
            Margin = ControlConstants.BaseMargin;
        }

        public void SoftUpdate(ThemeSet workspace)
        {
            _selected = false;
            _hoverOpacity = workspace.HoveredTextTransparency;
            _letterForeground = workspace.Foreground;
            _selectedLetterForeground = workspace.SelectedForeground;

            foreach (LetterControl letterControl in Children)
            {
                letterControl.SetForeground(workspace);

                if (!Flag) letterControl.Foreground.Opacity = workspace.InactiveTextTransparency;
            }
        }

        public void SuperSoftUpdate(ThemeSet workspace)
        {
            _hoverOpacity = workspace.HoveredTextTransparency;
            _letterForeground = workspace.Foreground;
            _selectedLetterForeground = workspace.SelectedForeground;

            foreach (LetterControl letterControl in Children)
            {
                letterControl.SetForeground(workspace, _selected);

                if (!Flag) letterControl.Foreground.Opacity = workspace.InactiveTextTransparency;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}px ind:{2}:{3}",
                nameof(WordPanel),
                CalculatedWidth,
                Index,
                WordIndex);
        }

        private void SetLettersOpacity(double opacity = 1)
        {
            foreach (LetterControl letter in Children)
            {
                letter.Foreground.Opacity = opacity;
            }
        }

        private void SetLettersForeground(string hex)
        {
            foreach (LetterControl letter in Children)
            {
                double opacity = letter.Foreground.Opacity;

                letter.Foreground = hex.ToBrush();
                letter.Foreground.Opacity = opacity;
            }
        }

        private bool _mouseDown = false;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (Manager is null || !Flag || !IsTextType) return;

            if (e.ChangedButton == MouseButton.Left) _mouseDown = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (Manager is null || !Flag || !IsTextType) return;

            if (e.ChangedButton == MouseButton.Left)
            {
                if (_mouseDown)
                {
                    PerformMouseClick();
                    _mouseDown = false;
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                AppendPopup();
            }

            base.OnMouseUp(e);
        }

        public void PerformMouseClick()
        {
            if (Manager is null) return;

            _selected = !_selected;

            Manager.OnWordSelected(_selected, this);

            if (_selected) SetLettersForeground(_selectedLetterForeground);
            else SetLettersForeground(_letterForeground);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (Manager is null || !Flag || !IsTextType) return;

            SetLettersOpacity(_hoverOpacity);

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (Manager is null || !Flag || !IsTextType) return;

            SetLettersOpacity();
            _mouseDown = false;

            base.OnMouseLeave(e);
        }

        private FlexWordsDialog Manager
        {
            get
            {
                if (Dialog is null) throw new Exception();

                return Dialog;
            }
        }
        private Popup PopupMenu => Manager.popupTranslateContent;

        private void AppendPopup()
        {
            string textForTranslation = _selected ? Manager.SelectedText : Text;

            if (string.IsNullOrEmpty(textForTranslation)) return;

            PopupMenu.PlacementTarget = this;
            PopupMenu.DataContext.CheckDispose();
            PopupMenu.DataContext = new WordPopupViewModel(textForTranslation, this, Manager);
        }

        internal void Flickering(bool flag)
        {
            AnimHelper.Flickering(this, !flag);
        }
    }
}
