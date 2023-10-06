using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using FlexWords.Dialog.Animation;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media;
using FlexWords.Dialog.Helpers;
using System.Windows.Input;
using FlexWords.Dialog.Controls.Components;
using FlexWords.Dialog.Extensions;
using FlexWords.Helpers;

namespace FlexWords.Dialog.Controls.UserDialogs
{
    public partial class TextReplacementDialog : Window
    {
        private bool _inProgress = false;
        public readonly List<string> CurrentData = new();

        public TextReplacementDialog()
        {
            InitializeComponent();

            AnimHelper.TextHoverTo(closeDialogButton, Brushes.DarkGoldenrod);
            AnimHelper.TextHoverTo(openFileButton, Brushes.DarkGoldenrod);
            AnimHelper.TextHoverTo(saveFileButton, Brushes.DarkGoldenrod);
            AnimHelper.TextHoverTo(replaceButton, Brushes.DarkGoldenrod);
            AnimHelper.TextHoverTo(findButton, Brushes.DarkGoldenrod);
            AnimHelper.TextHoverTo(resetButton, Brushes.DarkGoldenrod);

            Loaded += OnDialogLoaded;
            closeDialogButton.MouseUp += OnCloseDialog;
            findButton.MouseUp += OnFindText;
            resetButton.MouseUp += OnResetText;
            openFileButton.MouseUp += OnOpenFile;
            saveFileButton.MouseUp += OnSaveFile;
            replaceButton.MouseUp += OnTextReplace;
            fromReplaceContainer.TextChanged += OnTextChanged;
            toReplaceContainer.TextChanged += OnTextChanged;
        }

        private void OnResetText(object sender, MouseButtonEventArgs e)
        {
            if (_inProgress) return;

            findTextContainer.Text = string.Empty;
            ResetText();
        }

        private void OnFindText(object sender, MouseButtonEventArgs e)
        {
            if (_inProgress) return;

            FindText(findTextContainer.Text);
        }

        private void FindText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                ResetText();
                return;
            }

            Dispatcher.InvokeAsync(() =>
            {
                _inProgress = true;
                SetProgress(0, CurrentData.Count);

                foreach (TextBlock textBlock in textContainer.Items)
                {
                    bool check = textBlock.Text.Contains(text);

                    if (textBlock.Visibility == Visibility.Visible && !check)
                    {
                        textBlock.Visibility = Visibility.Collapsed;
                    }
                    else if (textBlock.Visibility == Visibility.Collapsed && check)
                    {
                        textBlock.Visibility = Visibility.Visible;
                    }
                }

                SetProgress(CurrentData.Count, CurrentData.Count);
                _inProgress = false;
            }, System.Windows.Threading.DispatcherPriority.Render);
        }

        private void ResetText()
        {
            Dispatcher.InvokeAsync(() =>
            {
                _inProgress = true;
                SetProgress(0, CurrentData.Count);

                foreach (TextBlock textBlock in textContainer.Items)
                {
                    textBlock.Visibility = Visibility.Visible;
                }

                SetProgress(CurrentData.Count, CurrentData.Count);
                _inProgress = false;
            }, System.Windows.Threading.DispatcherPriority.Render);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(fromReplaceContainer.Text) ||
                fromReplaceContainer.Text.Length > 1)
            {
                fromReplaceCode.Text = string.Empty;
            }
            else
            {
                fromReplaceCode.Text = ((int)fromReplaceContainer.Text[0]).ToString();
            }

            if (string.IsNullOrEmpty(toReplaceContainer.Text) ||
                toReplaceContainer.Text.Length > 1)
            {
                toReplaceCode.Text = string.Empty;
            }
            else
            {
                toReplaceCode.Text = ((int)toReplaceContainer.Text[0]).ToString();
            }
        }

        private void OnTextReplace(object sender, MouseButtonEventArgs e)
        {
            if (_inProgress || string.IsNullOrEmpty(fromReplaceContainer.Text)) return;

            _inProgress = true;

            ReplaceText(fromReplaceContainer.Text, toReplaceContainer.Text);
            fromReplaceContainer.Text = string.Empty;
            toReplaceContainer.Text = string.Empty;
            UpdateDictionary();

            _inProgress = false;
        }

        private void ReplaceText(string from, string to)
        {
            for (int i = 0; i < CurrentData.Count; i++)
            {
                if (CurrentData[i].Contains(from))
                {
                    CurrentData[i] = CurrentData[i].Replace(from, to);
                }
            }

            SetProgress(0, CurrentData.Count);

            Dispatcher.InvokeAsync(async () =>
            {
                _inProgress = true;

                int counter = 1;
                int delay = 120;
                int baseCount = 40;
                int count = baseCount;

                foreach (TextBlock item in textContainer.Items)
                {
                    if (count == 0)
                    {
                        count = baseCount;
                        await Task.Delay(delay);
                    }

                    if (item.Text.Contains(from))
                    {
                        item.Text = item.Text.Replace(from, to);
                        count--;
                    }

                    SetProgress(counter++, CurrentData.Count);
                }

                SetProgress(CurrentData.Count, CurrentData.Count);

                _inProgress = false;
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            ThemeApplier.UpdateTitleBar(this, true);
        }

        private void UpdateDictionary()
        {
            dictContainer.Items.Clear();

            bool tableColor = false;
            var dict = DataHelper.GetDictOfSymbols(CurrentData.ToArray());

            foreach (var symbol in dict)
            {
                var item = new DictionaryItem();
                item.Text = symbol.Key;
                item.Count = symbol.Value;

                dictContainer.Items.Add(item);

                tableColor = !tableColor;
            }
        }

        private void AddText()
        {
            SetProgress(0, CurrentData.Count);
            textContainer.Items.Clear();

            Dispatcher.InvokeAsync(async () =>
            {
                _inProgress = true;

                int counter = 1;
                int delay = 60;
                int baseCount = 40;
                int count = baseCount;

                foreach (string line in CurrentData)
                {
                    if (count == 0)
                    {
                        count = baseCount;
                        await Task.Delay(delay);
                    }

                    textContainer.Items.Add(new TextBlock
                    {
                        Text = "   " + line,
                        Foreground = ColorExtensions.ToBrush("#5F6368"),
                        FontSize = 11,
                        TextWrapping = TextWrapping.Wrap,
                        FontFamily = new FontFamily("Verdana")
                    });

                    SetProgress(counter++, CurrentData.Count);

                    count--;
                }

                _inProgress = false;
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void SetProgress(int current, int max)
        {
            float interest = ((float)current / max) * 100f;

            textProgress.Text = $"{interest:f0}%";
        }

        private void OnOpenFile(object sender, RoutedEventArgs e)
        {
            if (_inProgress) return;

            string filter = "Text file (*.txt)|*.txt";
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var ofd = new OpenFileDialog
            {
                Multiselect = false,
                Filter = filter,
                InitialDirectory = folder
            };

            if (ofd.ShowDialog() ?? false)
            {
                string[] data = FileHelper.ReadAndRemoveEmptyLines(ofd.FileName);

                textNameContainer.Text = Path.GetFileNameWithoutExtension(ofd.FileName);

                CurrentData.Clear();
                CurrentData.AddRange(data);
                AddText();
                UpdateDictionary();
            }
        }

        private void OnSaveFile(object sender, RoutedEventArgs e)
        {
            if (_inProgress || CurrentData.Count == 0) return;

            string filter = "Text file (*.txt)|*.txt";
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var ofd = new SaveFileDialog
            {
                Filter = filter,
                InitialDirectory = folder
            };

            if (ofd.ShowDialog() ?? false)
            {
                File.WriteAllLines(ofd.FileName, CurrentData);
                CurrentData.Clear();
                AddText();
                UpdateDictionary();
                textNameContainer.Text = string.Empty;
                SetProgress(0, 1);
            }
        }

        private void OnCloseDialog(object sender, RoutedEventArgs e)
        {
            if (_inProgress) return;

            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            Loaded -= OnDialogLoaded;
            closeDialogButton.MouseUp -= OnCloseDialog;
            findButton.MouseUp -= OnFindText;
            resetButton.MouseUp -= OnResetText;
            openFileButton.MouseUp -= OnOpenFile;
            saveFileButton.MouseUp -= OnSaveFile;
            replaceButton.MouseUp -= OnTextReplace;
            fromReplaceContainer.TextChanged -= OnTextChanged;
            toReplaceContainer.TextChanged -= OnTextChanged;

            base.OnClosed(e);
        }
    }
}
