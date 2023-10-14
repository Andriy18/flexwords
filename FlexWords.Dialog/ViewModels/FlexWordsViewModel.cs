using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexWords.Dialog.Controls;
using FlexWords.Dialog.Extensions;

namespace FlexWords.Dialog.ViewModels
{
    public sealed class FlexWordsViewModel : ObservableObject
    {
        private readonly FlexWordsDialog _dialog;
        private Shape? _focusModeIcon;
        private bool _isCancelPopupOpened;
        private string? _chapterName;
        private string? _openedBookPageNumber;
        private string? _bookmarkText;
        private double _pageTurnerValue;
        private double _maxPageTurnerValue;
        private bool _isSettingsOpened;
        private bool _showFolderManager;

        public FlexWordsViewModel(FlexWordsDialog dialog)
        {
            _dialog = dialog;
            OnLockScrollClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnLockScrollClicked, IsBookOpened);
            OnTextContainerSizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(OnTextContainerSizeChanged);
            OnFocusModeClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnFocusModeClicked, IsBookOpened);
            OnFocusModeLoadedCommand = new RelayCommand<RoutedEventArgs>(OnFocusModeLoaded);
            OnBookmarkClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnBookmarkClicked, IsBookOpened);
            OnCancelPopupOpenCommand = new RelayCommand<MouseButtonEventArgs>(OnCancelPopupOpen, IsBookOpened);
            OnCancelPopupClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnCancelPopupClicked, IsBookOpened);
            OnPageTurnerClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnPageTurnerClicked, IsBookOpened);
            OnSettingsClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnSettingsClicked, IsBookOpened);
            OnShowFolderClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnShowFolderClicked);
        }

        public IRelayCommand OnTextContainerSizeChangedCommand { get; }
        public IRelayCommand OnLockScrollClickedCommand { get; }
        public IRelayCommand OnFocusModeClickedCommand { get; }
        public IRelayCommand OnFocusModeLoadedCommand { get; }
        public IRelayCommand OnBookmarkClickedCommand { get; }
        public IRelayCommand OnCancelPopupOpenCommand { get; }
        public IRelayCommand OnCancelPopupClickedCommand { get; }
        public IRelayCommand OnPageTurnerClickedCommand { get; }
        public IRelayCommand OnSettingsClickedCommand { get; }
        public IRelayCommand OnShowFolderClickedCommand { get; }

        public bool IsScrollLocked { get; set; }
        public bool FocusMode { get; set; }
        public bool IsBookmarkSaved { get; set; }
        public double CanclePopupSize { get; set; }
        public double CanclePopupIconSize { get; set; }
        public bool IsPageTurnerShowed { get; set; }
        public bool IsSettingsOpened
        {
            get => _isSettingsOpened;
            set => SetProperty(ref _isSettingsOpened, value);
        }
        public string? ChapterName
        {
            get => _chapterName;
            set => SetProperty(ref _chapterName, value);
        }
        public string? OpenedBookPageNumber
        {
            get => _openedBookPageNumber;
            set => SetProperty(ref _openedBookPageNumber, value);
        }
        public string? BookmarkText
        {
            get => _bookmarkText;
            set => SetProperty(ref _bookmarkText, value);
        }
        public double PageTurnerValue
        {
            get => _pageTurnerValue;
            set => SetProperty(ref _pageTurnerValue, value);
        }
        public double MaxPageTurnerValue
        {
            get => _maxPageTurnerValue;
            set => SetProperty(ref _maxPageTurnerValue, value);
        }
        public bool IsCancelPopupOpened
        {
            get => _isCancelPopupOpened;
            set
            {
                if (value)
                {
                    CanclePopupSize = Math.Clamp(FlexWordsDialog.GetCurrentFontHeight() * 0.8, 12, double.MaxValue);
                    CanclePopupIconSize = CanclePopupSize * 0.8;
                    OnPropertyChanged(nameof(CanclePopupSize));
                    OnPropertyChanged(nameof(CanclePopupIconSize));
                }

                SetProperty(ref  _isCancelPopupOpened, value);

                _dialog.SelectedWordsFlickering(value);
            }
        }
        public bool ShowFolderManager
        {
            get => _showFolderManager;
            set => SetProperty(ref _showFolderManager, value);
        }

        private void OnTextContainerSizeChanged(SizeChangedEventArgs? args)
        {
            if (args is null || !args.WidthChanged) return;

            _dialog.Reorder(_dialog.CurrentWorkspace);
        }

        private void OnLockScrollClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out Border _)) return;

            IsScrollLocked = !IsScrollLocked;
            OnPropertyChanged(nameof(IsScrollLocked));
        }

        private void OnFocusModeClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out Border _)) return;

            FocusMode = !FocusMode;
            OnPropertyChanged(nameof(FocusMode));
        }

        private void OnFocusModeLoaded(RoutedEventArgs? args)
        {
            if (!args.TryCheckNullArgs(out Border border)) return;

            _focusModeIcon = (Shape)border.Child;
            ApplyFocusModeTheme();
        }

        public void ApplyFocusModeTheme()
        {
            if (_focusModeIcon is null) return;

            _focusModeIcon.Fill = _dialog.CurrentWorkspace.Foreground.ToBrush();
        }

        private void OnBookmarkClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out Border _)) return;

            if (IsBookmarkSaved)
            {
                if (MessageBox.Show(
                    "Confirm to release capture of page number",
                    "Release capture of page number",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question) == MessageBoxResult.Cancel) return;

                Options.CaptureBkmark = null;
            }
            else
            {
                _dialog.SetBookmark();
            }

            IsBookmarkSaved = !IsBookmarkSaved;
            OnPropertyChanged(nameof(IsBookmarkSaved));
            _dialog.UpdateCaptureBookmarkView();
        }

        public void InitBookmark(bool value)
        {
            IsBookmarkSaved = value;
            OnPropertyChanged(nameof(IsBookmarkSaved));
        }

        private bool IsBookOpened(RoutedEventArgs? args)
        {
            return _dialog.IsBookOpened;
        }

        private void OnCancelPopupOpen(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckRightButtonNullArgs(out Grid _)) return;

            if (!_dialog.AreAnyTextSelected) return;

            IsCancelPopupOpened = true;
        }

        private void OnCancelPopupClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out Border _)) return;

            IsCancelPopupOpened = false;
            _dialog.UnselectAllWords();
        }

        private void OnPageTurnerClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out Border _)) return;

            IsPageTurnerShowed = !IsPageTurnerShowed;
            OnPropertyChanged(nameof(IsPageTurnerShowed));
        }

        private void OnSettingsClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out Border _)) return;

            _dialog.PerformSettingsClick(true);
        }

        private void OnShowFolderClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out Border _)) return;

            _dialog.PerformFolderClick(true);
        }
    }
}
