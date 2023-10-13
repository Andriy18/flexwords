using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
            InitializeDpi();
            ThemeApplier.UpdateTitleBar(this, true);
            InitializeSettings();
            InitializeButtons();

            if (File.Exists(Options.LastBookPath))
            {
                OpenBook(Options.LastBookPath);

                if (_openedBook is null) return;

                fileNameContainer.Text = Options.LastBookPath;
                _openedBook.Bkmark = Options.LastBkmark;

                ForceUpdate(_openedBook, CurrentWorkspace);
                __move_slider.Value = GetCurrentParagraph().TotalPages;
                UpdateBookmarkView();
            }

            ApplyLogoLoading();
            InitializeAnimation();
        }

        private void InitializeButtons()
        {
            moveContainer.Opacity = 0.05;
            moveContainer.Visibility = Visibility.Hidden;
            moveContainer.Tag = false;
            viewModel.InitBookmark(Options.CaptureBkmark is not null);
            UpdateCaptureBookmarkView();
            settingsContainer.Tag = false;

            sizeContainer.Text = $"{Width * scale.DpiScaleX}x{Height * scale.DpiScaleY}";
        }

        private void InitializeAnimation()
        {
            AnimHelper.GeometryOpacity(__copy_book_path, (Shape)__copy_book_path.Child);
            AnimHelper.GeometryOpacity(__copy_book_page, (Shape)__copy_book_page.Child);
            AnimHelper.GeometryOpacity(__lock_page_turner, (Shape)__lock_page_turner.Child);
            AnimHelper.GeometryOpacity(__setting_book, (Shape)__setting_book.Child);
            AnimHelper.GeometryOpacity(__copy_capture_page, (Shape)__copy_capture_page.Child);
        }

        private void OnMouseWheelChanged(object sender, MouseWheelEventArgs e)
        {
            if (_openedBook is null || viewModel.IsScrollLocked) return;

            if (e.Delta > 0) ForceUpdate(_openedBook, CurrentWorkspace, _openedBook.Back());
            else if (e.Delta < 0) ForceUpdate(_openedBook, CurrentWorkspace, _openedBook.Next());

            if (popupTranslateContent.IsOpen) popupTranslateContent.IsOpen = false;
            if (viewModel.IsCancelPopupOpened) viewModel.IsCancelPopupOpened = false;

            __move_slider.Value = GetCurrentParagraph().TotalPages;
            UpdateBookmarkView();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            sizeContainer.Text = $"{e.NewSize.Width * scale.DpiScaleX}x{e.NewSize.Height * scale.DpiScaleY}";

            if (e.WidthChanged) __move_slider.Width = moveContainer.Width - 60.0;
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

                fileNameContainer.Text = ofd.FileName;
                ForceUpdate(_openedBook, CurrentWorkspace);
                __move_slider.Value = GetCurrentParagraph().TotalPages;
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
            __move_slider.MaxValue = (int)(_openedBook.PageCount);
        }

        private void UpdateBookmarkView()
        {
            if (_openedBook is null) return;

            BookParagraph paragraph = GetCurrentParagraph();
            filePageContainer.Text = $"page {paragraph.TotalPages}/{(int)_openedBook.PageCount} ({paragraph.CurrentPageInterest * 100f:f0}%) {paragraph.Chapter.Title}";

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

                    __capture_pages.Text = $"turn {pages - currentPages:f2} pages forward";
                }
                else
                {
                    __capture_pages.Text = $"read {value:f2} pages";
                }
            }
            else
            {
                __capture_pages.Text = "no bookmark";
            }
        }

        private string GetCurrentPageProgress()
        {
            if (_openedBook is null) return string.Empty;

            BookParagraph paragraph = GetCurrentParagraph();

            return $"{paragraph.TotalPages + paragraph.CurrentPageInterest:f2}";
        }

        private string GetCapturePageProgress()
        {
            if (_openedBook is null) return string.Empty;

            if (Options.CaptureBkmark is not null)
            {
                float value = _openedBook.CalculateTotalPages(Options.CaptureBkmark.Value, _openedBook.Bkmark);

                return $"{value:f2}";
            }
            else return "0";
        }

        private BookParagraph GetCurrentParagraph()
        {
            if (_openedBook is null) throw new Exception("GetCurrentParagraph");

            return _openedBook.Paragraphs[_openedBook.Bkmark.paragraph];
        }

        private void OnMoveValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            if (moveContainer.Tag is bool value && value)
            {
                bool isSameParagraph = _openedBook.MoveToPage((float)__move_slider.Value);
                ForceUpdate(_openedBook, CurrentWorkspace, isSameParagraph);
                UpdateBookmarkView();
            }
        }

        private void OnMoveMouseEnter(object sender, MouseEventArgs e)
        {
            moveContainer.Tag = true;
            moveContainer.Opacity = 1;
        }

        private void OnMoveMouseLeave(object sender, MouseEventArgs e)
        {
            moveContainer.Tag = false;
            moveContainer.Opacity = 0.05;
        }

        private void OnPageTurnerLocked(object sender, MouseButtonEventArgs e)
        {
            bool flag = moveContainer.Visibility == 0;

            if (flag) moveContainer.Visibility = Visibility.Hidden;
            else moveContainer.Visibility = Visibility.Visible;

            flag = moveContainer.Visibility == 0;

            System.Windows.Shapes.Path path = (System.Windows.Shapes.Path)__lock_page_turner.Child;
            path.Fill = flag ? "#929292".ToBrush() : "#4B4C52".ToBrush();
            path.Data = (StreamGeometry)Application.Current.Resources[flag ? "lock-on" : "lock-off"];
        }

        private void OnPageInfoCopied(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(GetCurrentPageProgress(), TextDataFormat.UnicodeText);
            AnimHelper.DoubleFlashingTextBlock(filePageContainer);
        }

        private void OnCapturePageInfoCopied(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(GetCapturePageProgress(), TextDataFormat.UnicodeText);
            AnimHelper.DoubleFlashingTextBlock(__capture_pages);
        }

        private void OnBookPathCopied(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(fileNameContainer.Text, TextDataFormat.UnicodeText);
            AnimHelper.DoubleFlashingTextBlock(fileNameContainer);
        }

        private void OnSettingsBookClicked(object sender, MouseButtonEventArgs e)
        {
            PerformSettingsClick(true);
        }

        private void PerformSettingsClick(bool source = true)
        {
            if (settingsContainer.Tag is not bool flag || !(flag || source)) return;

            settingsContainer.Tag = flag = !flag;

            double newValue = flag ? 0 : 300;
            AnimHelper.RenderTransformMove(settingsContainer, newValue);

            System.Windows.Shapes.Path path = (System.Windows.Shapes.Path)__setting_book.Child;
            path.Fill = flag ? "#929292".ToBrush() : "#4B4C52".ToBrush();

            if (flag) this.MouseUp += OnSettingsMouseDown;
            else this.MouseUp -= OnSettingsMouseDown;
        }

        public void SetBookmark()
        {
            if (_openedBook is null) return;

            Options.CaptureBkmark = new Bookmark(_openedBook.Bkmark.paragraph, 0);
        }
    }
}
