using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FlexWords.Dialog.Animation;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.Helpers;

namespace FlexWords.Dialog.Controls.Components
{
    public partial class LiteColorPicker : UserControl
    {
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(Color),
                typeof(LiteColorPicker),
                new FrameworkPropertyMetadata(default(Color),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set
            {
                Color oldValue = (Color)GetValue(SelectedColorProperty);
                Color newValue = value;

                if (oldValue == newValue) return;

                SetValue(SelectedColorProperty, value);
            }
        }

        public LiteColorPicker()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == SelectedColorProperty)
            {
                UpdateTextSize((Color)e.NewValue);
            }

            base.OnPropertyChanged(e);
        }

        private void UpdateTextSize(Color color)
        {
            __text.FontSize = MeasureHelper.GetBestFontSize(color.ToHex(), __main_border.ActualWidth, __main_border.ActualHeight, 0.8);
        }

        private bool _clicked = false;

        private void OnLiteColorPickerLoaded(object sender, RoutedEventArgs e)
        {
            UpdateTextSize(SelectedColor);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (_clicked)
            {
                __color_picker.IsOpen = true;
                _clicked = false;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            _clicked = true;

            base.OnMouseDown(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.ChangedButton == 0)
            {
                AnimHelper.DoubleFlashingTextBlock(__text);
                Clipboard.SetText(SelectedColor.ToHex(), TextDataFormat.UnicodeText);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            double cornerRadius = sizeInfo.NewSize.Height / 2;
            double smallCornerRadius = cornerRadius - 3;

            __border.CornerRadius = new CornerRadius(cornerRadius);
            __internal_border.CornerRadius = new CornerRadius(smallCornerRadius);
            __main_border.CornerRadius = new CornerRadius(smallCornerRadius);

            base.OnRenderSizeChanged(sizeInfo);
        }
    }
}
