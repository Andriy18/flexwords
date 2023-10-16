using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexWords.Dialog.Controls;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.Helpers;

namespace FlexWords.Dialog.ViewModels
{
    public sealed class FlexWordsViewModel : ObservableObject
    {
        #region Readonly

        private readonly FlexWordsDialog _dialog;
        private readonly Dictionary<int, BitmapImage> _images = new();

        #endregion

        #region Fields

        private Shape? _focusModeIcon;
        private bool _isCancelPopupOpened;
        private string? _chapterName;
        private string? _openedBookPageNumber;
        private string? _bookmarkText;
        private double _pageTurnerValue;
        private double _maxPageTurnerValue;
        private bool _isSettingsOpened;
        private bool _showFolderManager;
        private BitmapImage? _coverImage;
        private int _themeIndex;
        private ComboBox? _themeBox;
        private Color _background;
        private Color _selectedForeground;
        private Color _foreground;
        private double _hoveredTextOpacity;
        private double _inactiveTextOpacity;
        private double _coverOpacity;
        private int _coverIndex;
        private bool _logoPreview;
        private bool _adaptiveWordSpacing;
        private bool _adaptiveKerning;
        private bool _autoCopySelectedText;
        private bool _useTranslator;
        private bool _useGoogleTranslate;
        private bool _useSpellChecker;
        private bool _addPronunciation;
        private bool _showSynonyms;
        private int _fontFamily;
        private int _fontWeight;
        private double _fontSize;
        private double _areaWidth;
        private double _verticalOffset;
        private double _horizontalOffset;
        private double _indent;
        private double _kerning;
        private double _wordSpacing;
        private double _lineSpacing;

        #endregion

        #region Initialize

        public FlexWordsViewModel(FlexWordsDialog dialog)
        {
            _dialog = dialog;
            OnLockScrollClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnLockScrollClicked, IsBookOpened);
            OnTextContainerSizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(OnTextContainerSizeChanged);
            OnFocusModeClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnFocusModeClicked);
            OnFocusModeLoadedCommand = new RelayCommand<RoutedEventArgs>(OnFocusModeLoaded);
            OnBookmarkClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnBookmarkClicked, IsBookOpened);
            OnCancelPopupOpenCommand = new RelayCommand<MouseButtonEventArgs>(OnCancelPopupOpen, IsBookOpened);
            OnCancelPopupClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnCancelPopupClicked, IsBookOpened);
            OnPageTurnerClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnPageTurnerClicked, IsBookOpened);
            OnSettingsClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnSettingsClicked);
            OnShowFolderClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnShowFolderClicked);
            OnLoadedAsyncCommand = new AsyncRelayCommand(OnLoadedAsync);
            OnThemeLoadedAsyncCommand = new AsyncRelayCommand<RoutedEventArgs>(OnThemeLoadedAsync);
            OnNewThemeClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnNewThemeClicked, IsBookOpened);
            OnDeleteThemeClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnDeleteThemeClicked, IsBookOpened);
            OnTextStyleResetCommand = new RelayCommand<MouseButtonEventArgs>(OnTextStyleReset, IsBookOpened);
            OnTextMarginResetCommand = new RelayCommand<MouseButtonEventArgs>(OnTextMarginReset, IsBookOpened);
            OnTextSpacingResetCommand = new RelayCommand<MouseButtonEventArgs>(OnTextSpacingReset, IsBookOpened);
            OnFontFamilyLoadedAsyncCommand = new AsyncRelayCommand<RoutedEventArgs>(OnFontFamilyLoadedAsync);
            OnFontWeightLoadedAsyncCommand = new AsyncRelayCommand<RoutedEventArgs>(OnFontWeightLoadedAsync);
        }

        #endregion

        #region Commands

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
        public IAsyncRelayCommand OnLoadedAsyncCommand { get; }
        public IAsyncRelayCommand OnThemeLoadedAsyncCommand { get; }
        public IRelayCommand OnNewThemeClickedCommand { get; }
        public IRelayCommand OnDeleteThemeClickedCommand { get; }
        public IRelayCommand OnTextStyleResetCommand { get; }
        public IRelayCommand OnTextMarginResetCommand { get; }
        public IRelayCommand OnTextSpacingResetCommand { get; }
        public IAsyncRelayCommand OnFontFamilyLoadedAsyncCommand { get; }
        public IAsyncRelayCommand OnFontWeightLoadedAsyncCommand { get; }

        #endregion

        #region Properties

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
            set
            {
                SetProperty(ref _pageTurnerValue, value);

                _dialog.UpdatePageTurner(IsPageTurnerShowed);
            }
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
        public BitmapImage? CoverImage
        {
            get => _coverImage;
            set => SetProperty(ref _coverImage, value);
        }
        public int ThemeIndex
        {
            get => _themeIndex;
            set
            {
                if (_dialog.CurrentWorkspace is not null)
                {
                    Options.UpdateTheme(_dialog.CurrentWorkspace);
                }

                SetProperty(ref _themeIndex, value);
                Options.ThemeIndex = value;

                _dialog.CurrentWorkspace = Options.GetTheme();
                SetThemeProperties(_dialog.CurrentWorkspace);
                _dialog.ForceUpdate();
            }
        }
        public Color Background
        {
            get => _background;
            set
            {
                SetProperty(ref _background, value);

                _dialog.CurrentWorkspace.Background = value.ToHex();
            }
        }
        public Color SelectedForeground
        {
            get => _selectedForeground;
            set
            {
                SetProperty(ref _selectedForeground, value);

                _dialog.CurrentWorkspace.SelectedForeground = value.ToHex();
                _dialog.SuperSoftUpdate();
            }
        }
        public Color Foreground
        {
            get => _foreground;
            set
            {
                SetProperty(ref _foreground, value);

                _dialog.CurrentWorkspace.Foreground = value.ToHex();
                _dialog.SuperSoftUpdate();
                ApplyFocusModeTheme();
            }
        }
        public double HoveredTextOpacity
        {
            get => _hoveredTextOpacity;
            set
            {
                SetProperty(ref _hoveredTextOpacity, value);

                _dialog.CurrentWorkspace.HoveredTextTransparency = value;
                _dialog.SuperSoftUpdate();
            }
        }
        public double InactiveTextOpacity
        {
            get => _inactiveTextOpacity;
            set
            {
                SetProperty(ref _inactiveTextOpacity, value);

                _dialog.CurrentWorkspace.InactiveTextTransparency = value;
                _dialog.SuperSoftUpdate();
            }
        }
        public double CoverOpacity
        {
            get => _coverOpacity;
            set
            {
                SetProperty(ref _coverOpacity, value);

                _dialog.CurrentWorkspace.CoverOpacity = value;
            }
        }
        public int CoverIndex
        {
            get => _coverIndex;
            set
            {
                SetProperty(ref _coverIndex, value);

                _dialog.CurrentWorkspace.CoverIndex = value;
                SetCoverImage();
            }
        }
        public bool LogoPreview
        {
            get => _logoPreview;
            set
            {
                SetProperty(ref _logoPreview, value);

                Options.LogoPreview = value;
            }
        }
        public bool AdaptiveWordSpacing
        {
            get => _adaptiveWordSpacing;
            set
            {
                SetProperty(ref _adaptiveWordSpacing, value);

                Options.AdaptiveWordSpacing = value;
                _dialog.Update();
            }
        }
        public bool AdaptiveKerning
        {
            get => _adaptiveKerning;
            set
            {
                SetProperty(ref _adaptiveKerning, value);

                Options.AdaptiveKerning = value;
                _dialog.Update();
            }
        }
        public bool AutoCopySelectedText
        {
            get => _autoCopySelectedText;
            set
            {
                SetProperty(ref _autoCopySelectedText, value);

                Options.AutoCopySelectedText = value;
            }
        }
        public bool UseTranslator
        {
            get => _useTranslator;
            set
            {
                SetProperty(ref _useTranslator, value);

                Options.UseTranslator = value;
            }
        }
        public bool UseGoogleTranslate
        {
            get => _useGoogleTranslate;
            set
            {
                SetProperty(ref _useGoogleTranslate, value);

                _dialog.Translator.UseGoogleTranslate = value;
                _dialog.__translator_name.Text = value
                    ? "Using Google Translate"
                    : "Using Reverso Translate";
                Options.UseGoogleTranslate = value;
            }
        }
        public bool UseSpellChecker
        {
            get => _useSpellChecker;
            set
            {
                SetProperty(ref _useSpellChecker, value);

                _dialog.Translator.CheckSpelling = value;
                Options.UseSpellChecker = value;
            }
        }
        public bool AddPronunciation
        {
            get => _addPronunciation;
            set
            {
                SetProperty(ref _addPronunciation, value);

                Options.AddPronunciation = value;
            }
        }
        public bool ShowSynonyms
        {
            get => _showSynonyms;
            set
            {
                SetProperty(ref _showSynonyms, value);

                Options.ShowSynonyms = value;
            }
        }
        public int FontFamily
        {
            get => _fontFamily;
            set
            {
                SetProperty(ref _fontFamily, value);

                Options.FontFamily = value;
                _dialog.Update();
            }
        }
        public int FontWeight
        {
            get => _fontWeight;
            set
            {
                SetProperty(ref _fontWeight, value);

                Options.FontWeight = value;
                _dialog.Update();
            }
        }
        public double FontSize
        {
            get => _fontSize;
            set
            {
                SetProperty(ref _fontSize, value);

                Options.FontSize = value;
                _dialog.Update();
            }
        }
        public double AreaWidth
        {
            get => _areaWidth;
            set
            {
                SetProperty(ref _areaWidth, value);

                Options.AreaWidth = value;
            }
        }
        public double VerticalOffset
        {
            get => _verticalOffset;
            set
            {
                SetProperty(ref _verticalOffset, value);

                Options.VerticalOffset = value;
                _dialog.textItemsContainer.Margin = MeasureHelper.GetMargin();
            }
        }
        public double HorizontalOffset
        {
            get => _horizontalOffset;
            set
            {
                SetProperty(ref _horizontalOffset, value);

                Options.HorizontalOffset = value;
                _dialog.textItemsContainer.Margin = MeasureHelper.GetMargin();
            }
        }
        public double Indent
        {
            get => _indent;
            set
            {
                SetProperty(ref _indent, value);

                Options.Indent = value;
                _dialog.Reorder();
            }
        }
        public double Kerning
        {
            get => _kerning;
            set
            {
                SetProperty(ref _kerning, value);

                Options.Kerning = value;
                _dialog.Update();
            }
        }
        public double WordSpacing
        {
            get => _wordSpacing;
            set
            {
                SetProperty(ref _wordSpacing, value);

                Options.WordSpacing = value;
                _dialog.Reorder();
            }
        }
        public double LineSpacing
        {
            get => _lineSpacing;
            set
            {
                SetProperty(ref _lineSpacing, value);

                Options.LineSpacing = value;
                _dialog.SuperSoftUpdate();
            }
        }

        #endregion

        #region Public

        public void InitBookmark(bool value)
        {
            IsBookmarkSaved = value;
            OnPropertyChanged(nameof(IsBookmarkSaved));
        }

        #endregion

        #region Private

        private void OnTextContainerSizeChanged(SizeChangedEventArgs? args)
        {
            if (args is null || !args.WidthChanged) return;

            _dialog.Reorder();
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
        private void ApplyFocusModeTheme()
        {
            if (_focusModeIcon is null || _dialog.CurrentWorkspace is null) return;

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
        private void SetCoverImage()
        {
            if (CoverIndex is 0)
            {
                CoverImage = null;

                return;
            }

            if (_images.ContainsKey(CoverIndex))
            {
                CoverImage = _images[CoverIndex];

                return;
            }

            _dialog.Dispatcher.BeginInvoke(() =>
            {
                var uri = new Uri($"../Images/background-{CoverIndex}.jpg", UriKind.Relative);
                var image = new BitmapImage(uri);
                _images.Add(CoverIndex, image);
                CoverImage = _images[CoverIndex];
            }, DispatcherPriority.Send);
        }
        private Task OnLoadedAsync()
        {
            LogoPreview = Options.LogoPreview;
            AdaptiveWordSpacing = Options.AdaptiveWordSpacing;
            AdaptiveKerning = Options.AdaptiveKerning;
            AutoCopySelectedText = Options.AutoCopySelectedText;
            UseTranslator = Options.UseTranslator;
            UseGoogleTranslate = Options.UseGoogleTranslate;
            UseSpellChecker = Options.UseSpellChecker;
            AddPronunciation = Options.AddPronunciation;
            ShowSynonyms = Options.ShowSynonyms;
            FontSize = Options.FontSize;
            AreaWidth = Options.AreaWidth;
            VerticalOffset = Options.VerticalOffset;
            HorizontalOffset = Options.HorizontalOffset;
            Indent = Options.Indent;
            Kerning = Options.Kerning;
            WordSpacing = Options.WordSpacing;
            LineSpacing = Options.LineSpacing;

            return Task.CompletedTask;
        }
        private Task OnThemeLoadedAsync(RoutedEventArgs? args)
        {
            if (!args.TryCheckNullArgs(out ComboBox box)) return Task.CompletedTask;

            _themeBox = box;
            UpdateThemeBox();
            ThemeIndex = Options.ThemeIndex;

            return Task.CompletedTask;
        }
        private void UpdateThemeBox()
        {
            if (_themeBox is null) return;

            _themeBox.Items.Clear();

            for (int i = 0; i < Options.Themes.Length; i++)
            {
                _themeBox.Items.Add(FlexWordsDialog.InstantiateBlock($"Theme {i + 1}"));
            }
        }
        private void SetThemeProperties(ThemeSet theme)
        {
            Background = theme.Background.ToColor();
            SelectedForeground = theme.SelectedForeground.ToColor();
            Foreground = theme.Foreground.ToColor();
            HoveredTextOpacity = theme.HoveredTextTransparency;
            InactiveTextOpacity = theme.InactiveTextTransparency;
            CoverOpacity = theme.CoverOpacity;
            CoverIndex = theme.CoverIndex;
        }
        private void OnDeleteThemeClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out TextBlock _)) return;

            _ = Options.DeleteTheme();
            UpdateThemeBox();
            ThemeIndex = Math.Clamp(ThemeIndex - 1, 0, int.MaxValue);
        }
        private void OnNewThemeClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out TextBlock _)) return;

            var themes = Options.AddTheme();
            UpdateThemeBox();
            ThemeIndex = themes.Length - 1;
        }
        private void OnTextSpacingReset(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out TextBlock _)) return;

            Indent = 45;
            Kerning = 4;
            WordSpacing = 15;
            LineSpacing = 8;
        }
        private void OnTextMarginReset(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out TextBlock _)) return;

            AreaWidth = 600;
            HorizontalOffset = 0;
            VerticalOffset = 0;
        }
        private void OnTextStyleReset(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out TextBlock _)) return;

            FontSize = 21;
            FontFamily = FontFamilyHelper.GetIndex("Verdana");
            FontWeight = FontWeightHelper.GetIndex(FontWeights.Normal);
        }
        private Task OnFontWeightLoadedAsync(RoutedEventArgs? args)
        {
            if (!args.TryCheckNullArgs(out ComboBox box)) return Task.CompletedTask;

            InitializeFontWeight(box);
            FontWeight = Options.FontWeight;

            return Task.CompletedTask;
        }
        private Task OnFontFamilyLoadedAsync(RoutedEventArgs? args)
        {
            if (!args.TryCheckNullArgs(out ComboBox box)) return Task.CompletedTask;

            InitializeFontFamily(box);
            FontFamily = Options.FontFamily;

            return Task.CompletedTask;
        }
        private void InitializeFontFamily(ComboBox box)
        {
            foreach (var family in FontFamilyHelper.FontFamilyNames)
            {
                var block = new TextBlock
                {
                    Text = family.ToString(),
                    FontFamily = FontFamilyHelper.GetFontFamily(family)
                };

                box.Items.Add(block);
            }
        }
        private void InitializeFontWeight(ComboBox box)
        {
            foreach (var weight in FontWeightHelper.FontWeightNames)
            {
                var block = new TextBlock
                {
                    Text = weight.ToString(),
                    FontWeight = FontWeightHelper.GetFontWeight(weight)
                };

                box.Items.Add(block);
            }
        }

        #endregion
    }
}
