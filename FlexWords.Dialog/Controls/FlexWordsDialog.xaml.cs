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

            if (File.Exists(Options.LastUsedBook))
            {
                OpenBookInternal(Options.LastUsedBook);

                if (_openedBook is null) return;

                _openedBook.Bkmark = Options.LastBkmark;

                ForceUpdate(_openedBook, CurrentWorkspace);
                viewModel.PageTurnerValue = GetCurrentParagraph().TotalPages;
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

            viewModel.PageTurnerValue = GetCurrentParagraph().TotalPages;
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

        public void OpenBook(string path)
        {
            if (IsBookOpened)
            {
                if (MessageBox.Show(
                    "Do you accept to delete last opened book progress?",
                    "Delete book progress",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question) == MessageBoxResult.Cancel) return;
            }

            OpenBookInternal(path);

            if (_openedBook is null) return;

            ForceUpdate(_openedBook, CurrentWorkspace);
            viewModel.PageTurnerValue = GetCurrentParagraph().TotalPages;
            Options.CaptureBkmark = null;
            viewModel.InitBookmark(false);
            UpdateBookmarkView();
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

        private void UpdateBookmarkView()
        {
            if (!IsBookOpened) return;

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
                bool isSameParagraph = _openedBook.MoveToPage((float)viewModel.PageTurnerValue);
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
    }
}
