using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.Helpers;

namespace FlexWords.Dialog.Controls
{
    public partial class FlexWordsDialog
    {
        private bool _mouseOverSettings = false;

        public Workspace CurrentWorkspace { get; private set; } = GetCurrentWorkspace();

        private void InitializeSettings()
        {
            if (Options.SetDefaultValues)
            {
                Options.SetDefault1();
                Options.SetDefault2();
                Options.SetDefault3();

                Options.SetDefaultValues = false;
            }

            InitializeSettingButtons();
            SetActiveSettingButtons();
            InitializeSettingFeatures();

            // 'General' options
            __logoPreview.Checked = Options.LogoPreview;
            __adaptiveWordSpacing.Checked = Options.AdaptiveWordSpacing;
            __adaptiveKerning.Checked = Options.AdaptiveKerning;
            __autoCopySelectedText.Checked = Options.AutoCopySelectedText;

            // 'Translator' options
            __useTranslator.Checked = Options.UseTranslator;
            __useGoogleTranslate.Checked = Options.UseGoogleTranslate;
            __useSpellChecker.Checked = Options.UseSpellChecker;
            __addPronunciation.Checked = Options.AddPronunciation;
            __showSynonyms.Checked = Options.ShowSynonyms;

            // 'Font style' options
            __fontSize.Value = Options.FontSize;
            __fontFamily.SelectedIndex = Options.FontFamily;
            __fontWeight.SelectedIndex = Options.FontWeight;

            // 'Text margin' options
            __areaWidth.Value = Options.AreaWidth;
            __verticalOffset.Value = Options.VerticalOffset;
            __horizontalOffset.Value = Options.HorizontalOffset;

            // 'Text spacing' options
            __indent.Value = Options.Indent;
            __kerning.Value = Options.Kerning;
            __wordSpacing.Value = Options.WordSpacing;
            __lineSpacing.Value = Options.LineSpacing;

            // 'Color set 1' options
            // 'Color set 2' options

            FillComboBoxesData();

            // workspace options
            ApplyColorSetSettings(CurrentWorkspace);
        }

        private void FillComboBoxesData()
        {
            foreach (var family in FontFamilyHelper.FontFamilyNames)
            {
                var block = new TextBlock
                {
                    Text = family.ToString(),
                    FontFamily = FontFamilyHelper.GetFontFamily(family)
                };

                __fontFamily.Items.Add(block);
            }

            foreach (var weight in FontWeightHelper.FontWeightNames)
            {
                var block = new TextBlock
                {
                    Text = weight.ToString(),
                    FontWeight = FontWeightHelper.GetFontWeight(weight)
                };

                __fontWeight.Items.Add(block);
            }
        }

        private void InitializeSettingButtons()
        {
            TextBlock[] textBlocks =
            {
                __general, //0
                __translator, //1
                __font_style, //2
                __text_margin, //3
                __text_spacing, //4
                __color_set_1, //5
                __color_set_2, //6
            };

            for (int i = 0; i < textBlocks.Length - 2; i++)
            {
                int value = i;
                textBlocks[i].MouseUp += (s, e) => SetActiveSettingButtons(value);
            }

            textBlocks[5].MouseUp += (s, e) =>
            {
                Options.LastWorkspaceIndex = 0;
                SetActiveSettingButtons(5);
                CurrentWorkspace = GetCurrentWorkspace();
                ApplyColorSetSettings(CurrentWorkspace);
            };

            textBlocks[6].MouseUp += (s, e) =>
            {
                Options.LastWorkspaceIndex = 1;
                SetActiveSettingButtons(6);
                CurrentWorkspace = GetCurrentWorkspace();
                ApplyColorSetSettings(CurrentWorkspace);
            };
        }

        private void SetActiveSettingButtons(int buttonIndex = 0)
        {
            Brush active = "#E5E5E5".ToBrush();
            Brush unactibe = "#5F6368".ToBrush();
            Brush selected = "#92E492".ToBrush();

            TextBlock[] textBlocks =
            {
                __general, //0
                __translator, //1
                __font_style, //2
                __text_margin, //3
                __text_spacing, //4
                __color_set_1, //5
                __color_set_2, //6
            };

            for (int i = 0; i < textBlocks.Length; i++) textBlocks[i].Foreground = unactibe;
            textBlocks[Options.LastWorkspaceIndex + (textBlocks.Length - 2)].Foreground = active;
            textBlocks[buttonIndex].Foreground = selected;

            Grid[] grids =
            {
                __general_grid, //0
                __translator_grid, //1
                __font_style_grid, //2
                __text_margin_grid, //3
                __text_spacing_grid, //4
                __color_set_grid, //5
            };

            if (buttonIndex == (textBlocks.Length - 1)) buttonIndex = (textBlocks.Length - 2);
            for (int i = 0; i < grids.Length; i++) grids[i].Visibility = Visibility.Hidden;
            grids[buttonIndex].Visibility = Visibility.Visible;
        }

        private void ApplyColorSetSettings(Workspace workspace)
        {
            CurrentWorkspace = workspace;

            __background.SelectedColor = workspace.Background.ToColor();
            __foreground.SelectedColor = workspace.Foreground.ToColor();
            __selectedForeground.SelectedColor = workspace.SelectedForeground.ToColor();
            __hoveredTextTransparency.Value = workspace.HoveredTextTransparency;
            __inactiveTextTransparency.Value = workspace.InactiveTextTransparency;
        }

        private void InitializeSettingFeatures()
        {
            settingsContainer.MouseEnter += (s, e) => _mouseOverSettings = true;
            settingsContainer.MouseLeave += (s, e) => _mouseOverSettings = false;
        }

        private void OnSettingsMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_mouseOverSettings) PerformSettingsClick(false);
        }

        private void OnTextStyleReset(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MessageBox.Show(
                "Do you accept to reset Font style?",
                "Reset font style",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question) == MessageBoxResult.Cancel) return;

                Options.SetDefault1();

                __fontSize.Value = Options.FontSize;
                __fontFamily.SelectedIndex = Options.FontFamily;
                __fontWeight.SelectedIndex = Options.FontWeight;
            }
            finally
            {
                _mouseOverSettings = true;
            }
        }

        private void OnTextMarginReset(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MessageBox.Show(
                "Do you accept to reset Font margin?",
                "Reset font margin",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question) == MessageBoxResult.Cancel) return;

                Options.SetDefault2();

                __areaWidth.Value = Options.AreaWidth;
                __verticalOffset.Value = Options.VerticalOffset;
                __horizontalOffset.Value = Options.HorizontalOffset;
            }
            finally
            {
                _mouseOverSettings = true;
            }
        }

        private void OnTextSpacingReset(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MessageBox.Show(
                "Do you accept to reset Font spacing?",
                "Reset font spacing",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question) == MessageBoxResult.Cancel) return;

                Options.SetDefault3();

                __indent.Value = Options.Indent;
                __kerning.Value = Options.Kerning;
                __wordSpacing.Value = Options.WordSpacing;
                __lineSpacing.Value = Options.LineSpacing;
            }
            finally
            {
                _mouseOverSettings = true;
            }
        }

        private void OnColorSetReset(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MessageBox.Show(
                "Do you accept to reset current Color set?",
                "Reset color set",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question) == MessageBoxResult.Cancel) return;

                CurrentWorkspace = Workspace.Default;
                SaveWorkspace(CurrentWorkspace);
                ApplyColorSetSettings(CurrentWorkspace);
            }
            finally
            {
                _mouseOverSettings = true;
            }
        }

        private void OnColorSetSave(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MessageBox.Show(
                "Do you accept to save current Color set?",
                "Save color set",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question) == MessageBoxResult.Cancel) return;

                SaveWorkspace(CurrentWorkspace);
            }
            finally
            {
                _mouseOverSettings = true;
            }
        }

        private void OnLogoPreviewValueChanged(object sender, RoutedEventArgs e)
        {
            Options.LogoPreview = __logoPreview.Checked;
        }

        private void OnAdaptiveWordSpacingValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.AdaptiveWordSpacing = __adaptiveWordSpacing.Checked;
            Update(CurrentWorkspace);
        }

        private void OnAdaptiveKerningValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.AdaptiveKerning = __adaptiveKerning.Checked;
            Update(CurrentWorkspace);
        }

        private void OnAutoCopySelectedTextValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.AutoCopySelectedText = __autoCopySelectedText.Checked;
        }

        private void OnUseTranslatorValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.UseTranslator = __useTranslator.Checked;
        }

        private void OnUseGoogleTranslateValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.UseGoogleTranslate = __useGoogleTranslate.Checked;
            Translator.UseGoogleTranslate = __useGoogleTranslate.Checked;
        }

        private void OnUseSpellCheckerValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.UseSpellChecker = __useSpellChecker.Checked;
            Translator.CheckSpelling = __useSpellChecker.Checked;
        }

        private void OnAddPronunciationValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.AddPronunciation = __addPronunciation.Checked;
        }

        private void OnShowSynonymsValueChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.ShowSynonyms = __showSynonyms.Checked;
        }

        private void OnBackgroundColorChanged(object sender, RoutedEventArgs e)
        {
            CurrentWorkspace.Background = __background.SelectedColor.ToHex();

            this.Background = CurrentWorkspace.Background.ToBrush();
            UpdatePopupBackground(CurrentWorkspace);
        }

        private void OnForegroundColorChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            CurrentWorkspace.Foreground = __foreground.SelectedColor.ToHex();
            SuperSoftUpdate(CurrentWorkspace);
            UpdatePopupForeground(CurrentWorkspace);
            viewModel.ApplyFocusModeTheme();
        }

        private void OnSelectedForegroundColorChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            CurrentWorkspace.SelectedForeground = __selectedForeground.SelectedColor.ToHex();
            SuperSoftUpdate(CurrentWorkspace);
            UpdatePopupForeground(CurrentWorkspace);
        }

        private void OnFontFamilyChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_openedBook is null) return;

            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TextBlock block)
            {
                Options.FontFamily = FontFamilyHelper.GetIndex(block.Text);
                Update(CurrentWorkspace);
            }
        }

        private void OnFontWeightChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_openedBook is null) return;

            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TextBlock block)
            {
                Options.FontWeight = FontWeightHelper.GetIndex(block.Text);
                Update(CurrentWorkspace);
            }
        }

        private void OnFontSizeChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.FontSize = __fontSize.Value;
            Update(CurrentWorkspace);
        }

        private void OnAreaWidthChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.AreaWidth = __areaWidth.Value;
            textItemsContainer.Width = __areaWidth.Value;
            Reorder(CurrentWorkspace);
        }

        private void OnVerticalOffsetChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.VerticalOffset = __verticalOffset.Value;
            textItemsContainer.Margin = MeasureHelper.GetMargin();
        }

        private void OnHorizontalOffsetChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.HorizontalOffset = __horizontalOffset.Value;
            textItemsContainer.Margin = MeasureHelper.GetMargin();
        }

        private void OnIndentChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.Indent = __indent.Value;
            Reorder(CurrentWorkspace);
        }

        private void OnKerningChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.Kerning = __kerning.Value;
            Update(CurrentWorkspace);
        }

        private void OnWordSpacingChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.WordSpacing = __wordSpacing.Value;
            Reorder(CurrentWorkspace);
        }

        private void OnLineSpacingChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;

            Options.LineSpacing = __lineSpacing.Value;
            SuperSoftUpdate(CurrentWorkspace);
        }

        private void OnHoveredTextTransparencyChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;
            
            CurrentWorkspace.HoveredTextTransparency = __hoveredTextTransparency.Value;
            SuperSoftUpdate(CurrentWorkspace);

        }

        private void OnInactiveTextTransparencyChanged(object sender, RoutedEventArgs e)
        {
            if (_openedBook is null) return;
            
            CurrentWorkspace.InactiveTextTransparency = __inactiveTextTransparency.Value;
            SuperSoftUpdate(CurrentWorkspace);
        }

        private void UpdatePopupBackground(Workspace workspace)
        {
            __popup_border.Background = workspace.Background.ToBrush().AdaptiveAdjustBrightness(0.05);
            __popup_border.BorderBrush = workspace.Foreground.ToBrush().AdaptiveAdjustBrightness(0.25);
            __popup_part_of_border.Fill = __popup_border.Background;
            __popup_part_of_border.Stroke = __popup_border.BorderBrush;
            __popup_separator.Background = __popup_border.BorderBrush;
        }

        private void UpdatePopupForeground(Workspace workspace)
        {
            __popup_top_text.Foreground = workspace.Foreground.ToBrush();
            __popup_bottom_text.Foreground = workspace.Foreground.ToBrush().AdaptiveAdjustBrightness(0.25);
            ((Shape)__popup_top_copy.Child).Fill = __popup_bottom_text.Foreground;
            ((Shape)__popup_bottom_copy.Child).Fill = __popup_bottom_text.Foreground;
        }
    }
}
