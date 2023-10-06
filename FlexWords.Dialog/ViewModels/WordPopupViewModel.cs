using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FlexWords.Translator;
using FlexWords.Dialog.Handlers;
using FlexWords.Dialog.Helpers;
using FlexWords.Dialog.Animation;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.Controls;

namespace FlexWords.Dialog.ViewModels
{
    public sealed class WordPopupViewModel : ObservableObject, IDisposable
    {
        private readonly ITranslatorHandler _translator;
        private readonly Dispatcher _dispatcher;
        private readonly FlexWordsDialog _dialog;
        private readonly WordPanel _wordPanel;
        private readonly Workspace _workspace;
        private readonly string _text;

        private bool _showSynonyms;
        private bool _useTranslator;
        private bool _addPronunciation;
        private bool _isOpen;
        private bool _play;
        private double _fontSize;
        private double _smallFontSize;
        private double _copyTextHeigth;
        private Thickness _copyTextMargin;
        private Thickness _copyTranslatedTextMargin;
        private GridLength _row;
        private GridLength _middleRow;
        private MP3SoundPlayer? _player;

        public WordPopupViewModel(string textToTranslate, WordPanel wordPanel, FlexWordsDialog dialog)
        {
            _text = textToTranslate;
            _dialog = dialog;
            _dispatcher = dialog.Dispatcher;
            _translator = dialog.Translator;
            _wordPanel = wordPanel;
            _workspace = _dialog.CurrentWorkspace;
            UseTranslator = Options.UseTranslator;
            AddPronunciation = Options.AddPronunciation;
            ShowSynonyms = Options.ShowSynonyms;
            FontSize = Options.FontSize;
            
            _dialog.__popup_border.SizeChanged += OnSizeChanged;
            _dialog.__popup_top_copy.MouseUp += OnTopTextCopy;
            _dialog.__popup_bottom_copy.MouseUp += OnBottomTextCopy;
            _dialog.__popup_top_text.MouseUp += OnPlayAudio;

            _dialog.__popup_bottom_text.Inlines.Clear();
            _dialog.__popup_top_text.Text = _text;

            CollapsePopup();
            InitializeTranslation();
            InitializePronunciation();
            InitializeSynonyms();
            IsOpen = true;
        }

        private bool IsTranslationSuccessful => TranslatedText is not null;
        private string? TranslatedText
        {
            get
            {
                if (_dialog.__popup_bottom_text.Inlines.Count is 0) return null;

                string text = ((Run)_dialog.__popup_bottom_text.Inlines.FirstInline).Text;

                if (string.IsNullOrEmpty(text)) return null;

                return text;
            }
        }
        public bool ShowSynonyms
        {
            get => _showSynonyms;
            private set => SetProperty(ref _showSynonyms, value);
        }
        public bool UseTranslator
        {
            get => _useTranslator;
            private set => SetProperty(ref _useTranslator, value);
        }
        public bool AddPronunciation
        {
            get => _addPronunciation;
            private set => SetProperty(ref _addPronunciation, value);
        }
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }
        public bool Play
        {
            get => _play;
            set
            {
                _dispatcher.BeginInvoke(() =>
                {
                    _dialog.__popup_top_text.Foreground = value
                        ? _workspace.SelectedForeground.ToBrush()
                        : _workspace.Foreground.ToBrush();
                }, DispatcherPriority.Send);

                SetProperty(ref _play, value);
            }
        }
        public double FontSize
        {
            get => _fontSize;
            set
            {
                SmallFontSize = Math.Ceiling(value * 0.72);
                CopyTextHeigth = MeasureHelper.GetFontHeight(SmallFontSize * 0.58, "Verdana");

                double maxHeightTopText = MeasureHelper.GetFontHeight(value, "Verdana");
                double maxHeightBottomText = MeasureHelper.GetFontHeight(SmallFontSize, "Verdana");
                double differenceTopText = (maxHeightTopText - CopyTextHeigth) / 2;
                double differenceBottomText = (maxHeightBottomText - CopyTextHeigth) / 2;

                CopyTextMargin = new Thickness(0, differenceTopText, 6, 0);
                CopyTranslatedTextMargin = new Thickness(0, differenceBottomText, 6, 0);

                SetProperty(ref _fontSize, value);
            }
        }
        public double SmallFontSize
        {
            get => _smallFontSize;
            set => SetProperty(ref _smallFontSize, value);
        }
        public double CopyTextHeigth
        {
            get => _copyTextHeigth;
            set => SetProperty(ref _copyTextHeigth, value);
        }
        public Thickness CopyTextMargin
        {
            get => _copyTextMargin;
            set => SetProperty(ref _copyTextMargin, value);
        }
        public Thickness CopyTranslatedTextMargin
        {
            get => _copyTranslatedTextMargin;
            set => SetProperty(ref _copyTranslatedTextMargin, value);
        }
        public GridLength Row
        {
            get => _row;
            set => SetProperty(ref _row, value);
        }
        public GridLength MiddleRow
        {
            get => _middleRow;
            set => SetProperty(ref _middleRow, value);
        }

        private void OnPlayAudio(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            if (_player is null) return;

            Play = !Play;

            if (!Play) return;

            Task.Run(async () =>
            {
                _player.Play();

                while (_player.IsPlaying)
                {
                    if (!Play || !IsOpen)
                    {
                        _player.Stop();

                        break;
                    }
                }

                await _dispatcher.BeginInvoke(() => Play = false, DispatcherPriority.Send);
            });
        }
        private void OnTopTextCopy(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            Clipboard.SetText(_text, TextDataFormat.UnicodeText);
            AnimHelper.DoubleFlashingTextBlock(_dialog.__popup_top_text);
        }
        private void OnBottomTextCopy(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || !IsTranslationSuccessful) return;

            Clipboard.SetText(TranslatedText, TextDataFormat.UnicodeText);
            AnimHelper.DoubleFlashingTextBlock(_dialog.__popup_bottom_text);
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (!args.WidthChanged) return;

            double popupWidth = _dialog.__popup_border.ActualWidth;
            double wordWidth = _wordPanel.ActualWidth;
            double offset = Math.Abs(popupWidth - wordWidth) / 2;
            bool isPopupWider = popupWidth > wordWidth;

            // min value to do offset
            if (offset < 1.0) return;

            offset = isPopupWider ? -offset : offset;
            _dialog.popupTranslateContent.HorizontalOffset = offset;
        }

        private void InitializePronunciation()
        {
            if (!UseTranslator || !AddPronunciation) return;

            Task.Run(async () =>
            {
                MP3SoundPlayer player = await _translator.PronunciationAsync(_text);

                await _dispatcher.BeginInvoke((MP3SoundPlayer args) => _player = args,
                DispatcherPriority.Send,
                player);
            });
        }
        private void InitializeTranslation()
        {
            if (!UseTranslator) return;

            Task.Run(async () =>
            {
                string result = await _translator.TranslateAsync(_text);

                if (string.IsNullOrEmpty(result) || result == _text) return;

                await _dispatcher.BeginInvoke((string args) =>
                {
                    if (_dialog.__popup_bottom_text.Inlines.Count > 0)
                    {
                        ((Run)_dialog.__popup_bottom_text.Inlines.FirstInline).Text = args;
                    }
                    else _dialog.__popup_bottom_text.Inlines.Add(args);

                    if (IsTranslationSuccessful) ExpandPopup();
                },
                DispatcherPriority.Send,
                result);
            });
        }
        private void InitializeSynonyms()
        {
            if (!UseTranslator || !ShowSynonyms) return;

            Task.Run(async () =>
            {
                IEnumerable<Synonym> synonyms = await _translator.GetSynonymsAsync(_text);
                
                if (!synonyms.Any()) return;

                await _dispatcher.BeginInvoke((IEnumerable<Synonym> args) =>
                {
                    if (_dialog.__popup_bottom_text.Inlines.Count is 0)
                    {
                        _dialog.__popup_bottom_text.Inlines.Add(string.Empty);
                    }

                    _dialog.__popup_bottom_text.Inlines.Add(Environment.NewLine);
                    var run = new Run("Synonyms")
                    {
                        Foreground = _workspace.SelectedForeground.ToBrush().AdaptiveAdjustBrightness(0.1),
                        TextDecorations = new TextDecorationCollection { TextDecorations.Underline },
                        FontSize = Math.Clamp(SmallFontSize - 2, 1, SmallFontSize),
                    };
                    _dialog.__popup_bottom_text.Inlines.Add(run);
                    _dialog.__popup_bottom_text.Inlines.Add(Environment.NewLine);

                    string synonymsLine = string.Join(Environment.NewLine, synonyms.Select(i => "-" + i).Take(5));
                    run = new Run(synonymsLine)
                    {
                        Foreground = _workspace.SelectedForeground.ToBrush().AdaptiveAdjustBrightness(0.2),
                        FontStyle = FontStyles.Italic,
                        FontSize = Math.Clamp(SmallFontSize - 2, 1, SmallFontSize),
                    };
                    _dialog.__popup_bottom_text.Inlines.Add(run);
                },
                DispatcherPriority.Send,
                synonyms);
            });
        }

        private void ExpandPopup()
        {
            Row = GridLength.Auto;
            MiddleRow = new GridLength(9);
        }
        private void CollapsePopup()
        {
            Row = new GridLength(0);
            MiddleRow = new GridLength(0);
        }

        public void Dispose()
        {
            _dialog.__popup_border.SizeChanged -= OnSizeChanged;
            _dialog.__popup_top_copy.MouseUp -= OnTopTextCopy;
            _dialog.__popup_bottom_copy.MouseUp -= OnBottomTextCopy;
            _dialog.__popup_top_text.MouseUp -= OnPlayAudio;

            _player?.Dispose();
        }
    }
}
