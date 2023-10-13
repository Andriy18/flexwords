using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using FlexWords.Entities.Classes;
using FlexWords.Extensions;
using FlexWords.Translator;
using Microsoft.Win32;
using FlexWords.Dialog.Animation;
using FlexWords.Dialog.Controls.UserDialogs;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.Helpers;
using FlexWords.Dialog.ViewModels;
using FlexWords.Entities.Structs;

namespace FlexWords.Dialog.Controls
{
    public partial class FlexWordsDialog : Window
    {
        private readonly FlexWordsViewModel viewModel;
        private Book? _openedBook;

        public bool IsBookOpened => _openedBook is not null;

        public FlexWordsDialog()
        {
            InitializeComponent();
            viewModel = new FlexWordsViewModel(this);
            Translator = new TranslatorHandler();
            DataContext = viewModel;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ThemeApplier.UpdateTitleBar(this, true);
            InitializeSettings();
            InitializeButtons();

            if (File.Exists(Options.LastBookPath))
            {
                OpenBook(Options.LastBookPath);

                if (_openedBook is null) return;

                _openedBook.Bkmark = Options.LastBkmark;

                ForceUpdate(_openedBook, CurrentWorkspace);
                this.viewModel.PageTurnerValue = GetCurrentParagraph().TotalPages;
                UpdateBookmarkView();
            }

            ApplyLogoLoading();
        }

        private void InitializeButtons()
        {
            viewModel.InitBookmark(Options.CaptureBkmark is not null);
            UpdateCaptureBookmarkView();
        }

        private void OnMouseWheelChanged(object sender, MouseWheelEventArgs e)
        {
            if (_openedBook is null || viewModel.IsScrollLocked) return;

            if (e.Delta > 0) ForceUpdate(_openedBook, CurrentWorkspace, _openedBook.Back());
            else if (e.Delta < 0) ForceUpdate(_openedBook, CurrentWorkspace, _openedBook.Next());

            if (popupTranslateContent.IsOpen) popupTranslateContent.IsOpen = false;
            if (viewModel.IsCancelPopupOpened) viewModel.IsCancelPopupOpened = false;

            this.viewModel.PageTurnerValue = GetCurrentParagraph().TotalPages;
            UpdateBookmarkView();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            Translator?.Dispose();

            if (_openedBook is not null)
            {
                Options.LastBkmark = _openedBook.Bkmark;
            }
        }

        private void OnFileOpened(object sender, MouseButtonEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = false
            };

            if (ofd.ShowDialog() ?? false)
            {
                if (IsBookOpened)
                {
                    if (MessageBox.Show(
                        "Do you accept to delete last opened book progress?",
                        "Delete book progress",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Question) == MessageBoxResult.Cancel) return;
                }

                OpenBook(ofd.FileName);

                if (_openedBook is null) return;

                ForceUpdate(_openedBook, CurrentWorkspace);
                this.viewModel.PageTurnerValue = GetCurrentParagraph().TotalPages;
                Options.CaptureBkmark = null;
                viewModel.InitBookmark(false);
                UpdateBookmarkView();
            }
        }

        private void OnEdited(object sender, MouseButtonEventArgs e)
        {
            var dialog = new TextReplacementDialog();
            dialog.ShowDialog();
        }

        private void OnInfoShowed(object sender, MouseButtonEventArgs e)
        {
            string info = $"Build {CurrentVersion}v\n\n";

            if (IsBookOpened)
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

        private void OpenBook(string path)
        {
            var book = new Book(path);
            book.FillContent();
            book.FillStatistics();
            _openedBook = book;

            Options.LastBookPath = path;
            this.viewModel.MaxPageTurnerValue = (int)_openedBook.PageCount;
        }

        private void UpdateBookmarkView()
        {
            if (!IsBookOpened) return;

            BookParagraph paragraph = GetCurrentParagraph();
            this.viewModel.OpenedBookPageNumber = $"p: {ToDotString(paragraph.GetTotalPagesCount())}";
            this.viewModel.ChapterName = $"ch: {paragraph.Chapter.Title}";

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

                    this.viewModel.BookmarkText = $"-{ToDotString(pages - currentPages)}";
                }
                else
                {
                    this.viewModel.BookmarkText = $"{ToDotString(value)}";
                }
            }
            else
            {
                this.viewModel.BookmarkText = string.Empty;
            }
        }

        private BookParagraph GetCurrentParagraph()
        {
            if (_openedBook is null) throw new Exception("GetCurrentParagraph");

            return _openedBook.Paragraphs[_openedBook.Bkmark.paragraph];
        }

        private void OnMoveValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            if (viewModel.IsPageTurnerShowed)
            {
                bool isSameParagraph = _openedBook.MoveToPage((float)this.viewModel.PageTurnerValue);
                ForceUpdate(_openedBook, CurrentWorkspace, isSameParagraph);
                UpdateBookmarkView();
            }
        }

        public void PerformSettingsClick(bool source = true)
        {
            if (!viewModel.IsSettingsOpened && !source) return;

            viewModel.IsSettingsOpened = !viewModel.IsSettingsOpened;

            double newValue = viewModel.IsSettingsOpened ? 0 : 300;
            AnimHelper.RenderTransformMove(settingsContainer, newValue);

            if (viewModel.IsSettingsOpened) this.MouseUp += OnSettingsMouseDown;
            else this.MouseUp -= OnSettingsMouseDown;
        }

        public void SetBookmark()
        {
            if (_openedBook is null) return;

            Options.CaptureBkmark = new Bookmark(_openedBook.Bkmark.paragraph, 0);
        }
    }
}
