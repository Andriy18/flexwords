using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FlexWords.Entities.Classes;
using FlexWords.Extensions;
using FlexWords.Translator;
using FlexWords.Dialog.Animation;
using FlexWords.Dialog.Helpers;
using FlexWords.Dialog.ViewModels;
using FlexWords.Entities.Structs;
using FlexWords.Constants;
using System.Linq;
using FlexWords.Entities;
using FlexWords.FileExtractors;
using FlexWords.Entities.Enums;
using System.Diagnostics;
using System.Reflection;
using FlexWords.Dialog.Handlers;
using System.Collections.Generic;
using System.Windows.Controls;
using FlexWords.Dialog.Extensions;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Threading;

namespace FlexWords.Dialog.Controls
{
    public partial class FlexWordsDialog : Window
    {
        #region Readonly

        private readonly FlexWordsViewModel viewModel;
        private readonly Dictionary<Type, object> _dictStorage = new()
        {
            { typeof(LinePanel), new Stack<LinePanel>() },
            { typeof(GroupPanel), new Stack<GroupPanel>() },
            { typeof(WordPanel), new Stack<WordPanel>() },
            { typeof(LetterControl), new Stack<LetterControl>() },
        };
        private readonly List<WordPanel> _dictWords = new();

        #endregion

        #region Private Fields

        private Book? _openedBook;
        private bool _blockExecuteSettings;
        private bool _blockExecuteFolder;
        private bool _mouseOverSettings;
        private bool _mouseOverFolder;

        #endregion

        #region Public Properties

        public bool IsBookOpened => _openedBook is not null;
        public ThemeSet CurrentWorkspace { get; set; } = Options.GetTheme();
        public ITranslatorHandler Translator { get; }
        public string SelectedText
        {
            get => string.Join(' ', _dictWords.OrderBy(i => i.Index).Select(i => i.Text));
        }
        public bool AreAnyTextSelected => _dictWords.Count > 0;

        #endregion

        #region Event Handlers

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ThemeApplier.UpdateTitleBar(this, true);
            InitializeSettings();
            InitializeButtons();

            if (File.Exists(Options.LastUsedBook))
            {
                OpenBookInternal(Options.LastUsedBook);

                if (_openedBook is null) return;

                _openedBook.Bkmark = Options.LastBkmark;

                ForceUpdate();
                viewModel.PageTurnerValue = GetCurrentParagraph().TotalPages;
                UpdateBookmarkView();
            }

            ApplyLogoLoading();
        }
        private void OnMouseWheelChanged(object sender, MouseWheelEventArgs e)
        {
            if (_openedBook is null || viewModel.IsScrollLocked) return;

            if (e.Delta > 0) ForceUpdate(_openedBook.Back());
            else if (e.Delta < 0) ForceUpdate(_openedBook.Next());

            if (popupTranslateContent.IsOpen) popupTranslateContent.IsOpen = false;
            if (viewModel.IsCancelPopupOpened) viewModel.IsCancelPopupOpened = false;

            viewModel.PageTurnerValue = GetCurrentParagraph().TotalPages;
            UpdateBookmarkView();
        }
        private void OnInfoShowed(object sender, MouseButtonEventArgs e)
        {
            string info = $"Build {GetCurrentVersion()}v\n\n";

            if (_openedBook is not null)
            {
                info += $"Title: '{_openedBook?.Title}'\n";
                info += $"-Chapter count: {_openedBook?.Chapters.Count}\n";
                info += $"-Paragraph count: {_openedBook?.Paragraphs.Count}\n";
                info += $"-Sentence count: {_openedBook?.SentenseCount}\n";
                info += $"-Word count: {_openedBook?.WordCount}\n";
                info += $"Total {Math.Ceiling(_openedBook?.PageCount ?? 0)} pages\n";
            }

            MessageBox.Show(info, "General information");
        }
        private void OnWindowClosed(object sender, EventArgs e)
        {
            Translator?.Dispose();

            if (CurrentWorkspace is not null)
            {
                Options.UpdateTheme(CurrentWorkspace);
            }

            if (_openedBook is not null)
            {
                Options.LastBkmark = _openedBook.Bkmark;
            }
        }
        private void OnContainersMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_blockExecuteFolder)
            {
                _blockExecuteFolder = false;
            }
            else if (!_mouseOverFolder && viewModel.ShowFolderManager)
            {
                PerformFolderClick(false);
            }

            if (_blockExecuteSettings)
            {
                _blockExecuteSettings = false;
            }
            else if (!_mouseOverSettings && viewModel.IsSettingsOpened)
            {
                PerformSettingsClick(false);
            }
        }

        #endregion

        #region Initialize Data

        public FlexWordsDialog()
        {
            InitializeComponent();
            viewModel = new FlexWordsViewModel(this);
            Translator = new TranslatorHandler();
            DataContext = viewModel;
        }
        private void InitializeButtons()
        {
            viewModel.InitBookmark(Options.CaptureBkmark is not null);
            UpdateCaptureBookmarkView();
        }
        private void InitializeSettings()
        {
            TextBlock[] blocks = SettingsButtonContainer.Children
                .OfType<TextBlock>()
                .ToArray();

            for (int i = 0; i < blocks.Length; i++)
            {
                int value = i;
                blocks[i].MouseUp += (s, e) => SetActiveSettingButtons(value);
            }

            SetActiveSettingButtons();

            this.MouseUp += OnContainersMouseUp;

            settingsContainer.MouseEnter += (s, e) => _mouseOverSettings = true;
            settingsContainer.MouseLeave += (s, e) => _mouseOverSettings = false;

            folderContainer.MouseEnter += (s, e) => _mouseOverFolder = true;
            folderContainer.MouseLeave += (s, e) => _mouseOverFolder = false;
        }
        private void ApplyLogoLoading()
        {
            Dispatcher.BeginInvoke(async () =>
            {
                if (Options.LogoPreview)
                {
                    await Task.Delay(1000);

                    var animation1 = new DoubleAnimation(-0.2, TimeSpan.FromSeconds(3.0));
                    var animation2 = new DoubleAnimation(-0.1, TimeSpan.FromSeconds(3.0));
                    var animation3 = new DoubleAnimation(0.0, TimeSpan.FromSeconds(3.0));

                    __stop_1.BeginAnimation(GradientStop.OffsetProperty, animation1);
                    __stop_2.BeginAnimation(GradientStop.OffsetProperty, animation2);
                    __stop_3.BeginAnimation(GradientStop.OffsetProperty, animation3);

                    await Task.Delay(3500);
                    Brush brush = "#E5E5E5".ToBrush();
                    brush.Opacity = 0.0;
                    __temp_logo_path.Foreground = brush;
                    var animation4 = new DoubleAnimation(1.0, TimeSpan.FromSeconds(1.5));
                    animation4.EasingFunction = new BounceEase { EasingMode = EasingMode.EaseIn };
                    __temp_logo_path.Foreground.BeginAnimation(Brush.OpacityProperty, animation4);

                    await Task.Delay(200);
                    __temp_logo_name.BeginAnimation(TextBlock.OpacityProperty, animation4);

                    await Task.Delay(2000);
                }

                var animation = new DoubleAnimation(0, TimeSpan.FromSeconds(1.5));
                animation.EasingFunction = new PowerEase { EasingMode = EasingMode.EaseIn };
                __temp_logo_border.BeginAnimation(UIElement.OpacityProperty, animation);
                await Task.Delay(1500);
                __grid.Children.Remove(__temp_logo_border);
            }, DispatcherPriority.Send);
        }

        #endregion

        #region Public

        public void OpenBook(string path)
        {
            if (_openedBook is not null)
            {
                if (MessageBox.Show(
                    "Do you accept to delete last opened book progress?",
                    "Delete book progress",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question) == MessageBoxResult.Cancel) return;
            }

            OpenBookInternal(path);

            if (_openedBook is null) return;

            ForceUpdate();
            viewModel.PageTurnerValue = GetCurrentParagraph().TotalPages;
            Options.CaptureBkmark = null;
            viewModel.InitBookmark(false);
            UpdateBookmarkView();
        }
        public void UpdatePageTurner(bool isPageTurnerShowed)
        {
            if (_openedBook is null) return;

            if (isPageTurnerShowed)
            {
                bool isSameParagraph = _openedBook.MoveToPage((float)viewModel.PageTurnerValue);
                ForceUpdate(isSameParagraph);
                UpdateBookmarkView();
            }
        }
        public void UpdateBookmarkView()
        {
            if (_openedBook is null) return;

            BookParagraph paragraph = GetCurrentParagraph();
            viewModel.OpenedBookPageNumber = $"p: {ToDotString(paragraph.GetTotalPagesCount())}";
            viewModel.ChapterName = $"ch: {paragraph.Chapter.Title}";

            UpdateCaptureBookmarkView();
        }
        public void UpdateCaptureBookmarkView()
        {
            if (_openedBook is null) return;

            if (Options.CaptureBkmark is not null)
            {
                float value = _openedBook.CalculateTotalPages(Options.CaptureBkmark.Value, _openedBook.Bkmark);

                if (value < float.Epsilon)
                {
                    float currentPages = GetCurrentParagraph().GetTotalPagesCount();
                    float pages = _openedBook.Paragraphs[Options.CaptureBkmark.Value.paragraph].GetTotalPagesCount();

                    viewModel.BookmarkText = $"-{ToDotString(pages - currentPages)}";
                }
                else
                {
                    viewModel.BookmarkText = $"{ToDotString(value)}";
                }
            }
            else
            {
                viewModel.BookmarkText = string.Empty;
            }
        }
        public void PerformSettingsClick(bool source = true)
        {
            if (!viewModel.IsSettingsOpened && !source) return;

            viewModel.IsSettingsOpened = !viewModel.IsSettingsOpened;

            double newValue = viewModel.IsSettingsOpened ? 0 : 300;
            AnimHelper.RenderTransformMove(settingsContainer, newValue);

            if (source) _blockExecuteSettings = true;
        }
        public void PerformFolderClick(bool source = true)
        {
            if (!viewModel.ShowFolderManager && !source) return;

            viewModel.ShowFolderManager = !viewModel.ShowFolderManager;

            double newValue = viewModel.ShowFolderManager ? 0 : -300;
            AnimHelper.RenderTransformMove(folderContainer, newValue, true);

            if (source) _blockExecuteFolder = true;
        }
        public void SetBookmark()
        {
            if (_openedBook is null) return;

            Options.CaptureBkmark = new Bookmark(_openedBook.Bkmark.paragraph, 0);
        }
        public void SetActiveSettingButtons(int buttonIndex = 0)
        {
            Brush unactibe = "#5F6368".ToBrush();
            Brush selected = "#92E492".ToBrush();

            TextBlock[] blocks = SettingsButtonContainer.Children
                .OfType<TextBlock>()
                .ToArray();
            Grid[] grids = SettingsDataContainer.Children
                .OfType<Grid>()
                .ToArray();

            for (int i = 0; i < blocks.Length; i++) blocks[i].Foreground = unactibe;
            blocks[buttonIndex].Foreground = selected;

            for (int i = 0; i < grids.Length; i++) grids[i].Visibility = Visibility.Hidden;
            grids[buttonIndex].Visibility = Visibility.Visible;
        }
        public void SuperSoftUpdate()
        {
            if (CurrentWorkspace is null) return;

            SuperSoftRedrawLines(CurrentWorkspace);
        }
        public void Reorder()
        {
            if (CurrentWorkspace is null) return;

            ThemeSet theme = CurrentWorkspace;
            RedrawLines(Recompose(Options.AdaptiveKerning ? theme : null), theme);
        }
        public void Update()
        {
            if (CurrentWorkspace is null) return;

            ThemeSet theme = CurrentWorkspace;
            RedrawLines(Recompose(theme), theme);
        }
        public void ForceUpdate(bool isSameParagraph = false)
        {
            if (_openedBook is null || CurrentWorkspace is null) return;

            ThemeSet theme = CurrentWorkspace;

            if (isSameParagraph)
            {
                SoftRedrawLines(_openedBook, theme);
                _dictWords.Clear();

                return;
            }

            FullRecompose();
            RedrawLines(GenerateWordGroups(_openedBook, theme), theme);
            _dictWords.Clear();
        }
        public void UnselectAllWords()
        {
            string text = Clipboard.GetText(TextDataFormat.UnicodeText);

            while (AreAnyTextSelected)
            {
                _dictWords[^1].PerformMouseClick();
            }

            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }
        public void SelectedWordsFlickering(bool flag)
        {
            if (!AreAnyTextSelected) return;

            foreach (WordPanel panel in _dictWords)
            {
                panel.Flickering(flag);
            }
        }
        public void OnWordSelected(bool select, WordPanel panel)
        {
            if (select) _dictWords.Add(panel);
            else _dictWords.Remove(panel);

            if (!AreAnyTextSelected) return;

            if (Options.AutoCopySelectedText)
            {
                Clipboard.SetText(SelectedText, TextDataFormat.UnicodeText);
            }
        }

        #endregion

        #region Private

        private BookParagraph GetCurrentParagraph()
        {
            if (_openedBook is null) throw new Exception("GetCurrentParagraph");

            return _openedBook.Paragraphs[_openedBook.Bkmark.paragraph];
        }
        private void OpenBookInternal(string path)
        {
            FileFormat format = FileFormat.None;
            string ext = Path.GetExtension(path).ToLower();

            if (!Words.FileFormat.SupportedFormats.Contains(ext)) return;

            if (ext == Words.FileFormat.TxtExt) format = FileFormat.Txt;
            else if (ext == Words.FileFormat.EpubExt) format = FileFormat.Epub;

            RawFileData fileData = FileExtractor.Extract(path, format);

            if (!fileData.HasData) return;

            var book = new Book(path);
            book.FillContent(fileData);
            book.FillStatistics();
            _openedBook = book;

            Options.LastUsedBook = path;
            viewModel.MaxPageTurnerValue = (int)_openedBook.PageCount;
        }
        private T Instantiate<T>() where T : new()
        {
            var stack = (Stack<T>)_dictStorage[typeof(T)];

            if (stack.Count > 0) return stack.Pop();

            return new T();
        }
        private void Push<T>(T item) where T : new()
        {
            var list = (Stack<T>)_dictStorage[typeof(T)];

            list.Push(item);
        }

        #endregion

        #region Algorithms

        private IReadOnlyList<GroupPanel> GenerateWordGroups(Book book, ThemeSet workspace)
        {
            int wordIndex = 0;
            IReadOnlyList<WordItem[]> groups = book.CurrentWordGroups();
            GroupPanel[] result = new GroupPanel[groups.Count];

            for (int i = 0; i < groups.Count; i++)
            {
                GroupPanel groupPanel = Instantiate<GroupPanel>();
                result[i] = groupPanel;

                for (int j = 0; j < groups[i].Length; j++)
                {
                    WordItem item = groups[i][j];

                    WordPanel wordPanel = Instantiate<WordPanel>();
                    wordPanel.Dialog = this;
                    wordPanel.Flag = item.Flag;
                    wordPanel.Text = item.Value.Value ?? string.Empty;
                    wordPanel.IsTextType = item.Value.Type == WordType.Text;
                    groupPanel.Add(wordPanel, wordIndex++, j, workspace);

                    for (int k = 0; k < item.Letters.Length; k++)
                    {
                        LetterControl control = Instantiate<LetterControl>();
                        wordPanel.Add(control, item.Letters[k], k, workspace);
                    }
                }
            }

            return result;
        }
        private void RedrawLines(IReadOnlyList<GroupPanel> items, ThemeSet workspace)
        {
            int index = 0;
            int lineIndex = 0, groupIndex = 0;
            LinePanel linePanel = Instantiate<LinePanel>();
            linePanel.LineIndex = lineIndex++;
            linePanel.Readspace = workspace;
            textItemsContainer.Children.Add(linePanel);

            double maxWidth = textItemsContainer.ActualWidth;

            foreach (GroupPanel groupPanel in items)
            {
                if (linePanel.Children.Count is 0)
                {
                    groupPanel.Index = index++;
                    groupPanel.GroupIndex = groupIndex++;
                    groupPanel.Readspace = workspace;
                    linePanel.Children.Add(groupPanel);

                    continue;
                }

                if (maxWidth < linePanel.CalculatedWidth + groupPanel.CalculatedWidth + Options.WordSpacing)
                {
                    linePanel = Instantiate<LinePanel>();
                    linePanel.LineIndex = lineIndex++;
                    linePanel.Readspace = workspace;
                    textItemsContainer.Children.Add(linePanel);

                    groupIndex = 0;
                    groupPanel.Index = index++;
                    groupPanel.GroupIndex = groupIndex++;
                    groupPanel.Readspace = workspace;
                    linePanel.Children.Add(groupPanel);

                    continue;
                }

                groupPanel.Index = index++;
                groupPanel.GroupIndex = groupIndex++;
                groupPanel.Readspace = workspace;
                linePanel.Children.Add(groupPanel);
            }

            // AdaptiveKerning
            if (Options.AdaptiveKerning && !Options.AdaptiveWordSpacing)
            {
                for (int i = 0; i < textItemsContainer.Children.Count - 1; i++)
                {
                    linePanel = (LinePanel)textItemsContainer.Children[i];

                    if (linePanel.CalculatedWidth > maxWidth) continue;

                    double remainder = maxWidth - linePanel.CalculatedWidth;
                    double additionalLeftOffset = remainder / linePanel.KerningCount;

                    linePanel.SetAdditionalKerning(additionalLeftOffset);
                }

                return;
            }

            // AdaptiveKerning & AdaptiveWordSpacing
            if (Options.AdaptiveWordSpacing && Options.AdaptiveKerning)
            {
                for (int i = 0; i < textItemsContainer.Children.Count - 1; i++)
                {
                    linePanel = (LinePanel)textItemsContainer.Children[i];

                    if (linePanel.Children.Count < 2) continue;

                    int wordSpacingCount = linePanel.Children.Count - 1;
                    double remainder = maxWidth - linePanel.CalculatedWidth;

                    if (remainder < Options.WordSpacing * wordSpacingCount) continue;

                    remainder -= Options.WordSpacing * wordSpacingCount;
                    double additionalLeftOffset = remainder / linePanel.KerningCount;

                    linePanel.SetAdditionalKerning(additionalLeftOffset);
                }
            }

            if (!Options.AdaptiveWordSpacing) return;

            // AdaptiveWordSpacing
            for (int i = 0; i < textItemsContainer.Children.Count - 1; i++)
            {
                linePanel = (LinePanel)textItemsContainer.Children[i];
                double remainder = maxWidth - linePanel.CalculatedWidth;

                if (linePanel.Children.Count < 2) continue;

                double additionalLeftOffset = remainder / (linePanel.Children.Count - 1);

                for (int j = 1; j < linePanel.Children.Count; j++)
                {
                    GroupPanel groupPanel = (GroupPanel)linePanel.Children[j];
                    double leftMargin = groupPanel.Margin.Left + additionalLeftOffset;

                    groupPanel.Margin = new Thickness(leftMargin, 0, 0, 0);
                }
            }
        }
        private IReadOnlyList<GroupPanel> Recompose(ThemeSet? workspace = null)
        {
            int index = 0;
            GroupPanel groupPanel;
            LinePanel linePanel;
            int capacity = textItemsContainer.Children.Cast<LinePanel>().Sum(i => i.Children.Count);
            var result = new GroupPanel[capacity];

            while (textItemsContainer.Children.Count > 0)
            {
                linePanel = (LinePanel)textItemsContainer.Children[0];

                while (linePanel.Children.Count > 0)
                {
                    groupPanel = (GroupPanel)linePanel.Children[0];

                    if (workspace is not null)
                    {
                        foreach (WordPanel wordPanel in groupPanel.Children)
                        {
                            foreach (LetterControl letterControl in wordPanel.Children)
                            {
                                letterControl.Readspace = workspace;
                                letterControl.RecalculateWidth();
                            }

                            wordPanel.Readspace = workspace;
                        }
                    }

                    linePanel.Children.Remove(groupPanel);
                    groupPanel.Reset();
                    result[index++] = groupPanel;
                }

                textItemsContainer.Children.Remove(linePanel);
                linePanel.Reset();
                Push(linePanel);
            }

            return result;
        }
        private void FullRecompose()
        {
            LetterControl letterControl;
            WordPanel wordPanel;
            GroupPanel groupPanel;
            LinePanel linePanel;

            while (textItemsContainer.Children.Count > 0)
            {
                linePanel = (LinePanel)textItemsContainer.Children[0];

                while (linePanel.Children.Count > 0)
                {
                    groupPanel = (GroupPanel)linePanel.Children[0];

                    while (groupPanel.Children.Count > 0)
                    {
                        wordPanel = (WordPanel)groupPanel.Children[0];

                        while (wordPanel.Children.Count > 0)
                        {
                            letterControl = (LetterControl)wordPanel.Children[0];

                            wordPanel.Children.Remove(letterControl);
                            letterControl.Reset();
                            Push(letterControl);
                        }

                        groupPanel.Children.Remove(wordPanel);
                        wordPanel.Reset();
                        Push(wordPanel);
                    }

                    linePanel.Children.Remove(groupPanel);
                    groupPanel.Reset();
                    Push(groupPanel);
                }

                textItemsContainer.Children.Remove(linePanel);
                linePanel.Reset();
                Push(linePanel);
            }
        }
        private void SoftRedrawLines(Book book, ThemeSet workspace)
        {
            int index = 0;

            IReadOnlyList<WordItem> items = book.CurrentWords();

            foreach (LinePanel linePanel in textItemsContainer.Children)
            {
                foreach (GroupPanel groupPanel in linePanel.Children)
                {
                    foreach (WordPanel wordPanel in groupPanel.Children)
                    {
                        wordPanel.Flag = items[index++].Flag;
                        wordPanel.SoftUpdate(workspace);
                    }
                }

                linePanel.Readspace = CurrentWorkspace;
            }
        }
        private void SuperSoftRedrawLines(ThemeSet workspace)
        {
            foreach (LinePanel linePanel in textItemsContainer.Children)
            {
                foreach (GroupPanel groupPanel in linePanel.Children)
                {
                    foreach (WordPanel wordPanel in groupPanel.Children)
                    {
                        wordPanel.SuperSoftUpdate(workspace);
                    }
                }

                linePanel.Readspace = CurrentWorkspace;
            }
        }

        #endregion

        #region Static

        public static string? GetCurrentVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return fileVersionInfo.ProductVersion;
        }
        public static double GetCurrentFontHeight()
        {
            string name = FontFamilyHelper.GetFontFamilyName(Options.FontFamily);

            return MeasureHelper.GetFontHeight(Options.FontSize, name);
        }
        public static string ToDotString(float number)
        {
            return $"{(int)number}.{(int)((number % 1f) * 100f)}";
        }
        public static TextBlock InstantiateBlock(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontFamily = FontFamilyHelper.GetFontFamily("Verdana")
            };
        }

        #endregion
    }
}
