using System;
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
            DependencyProperty.RegisterAttached(
                nameof(SelectedColor),
                typeof(Color),
                typeof(LiteColorPicker));

        public static readonly RoutedEvent SelecteColorChangedRoutedEvent =
           EventManager.RegisterRoutedEvent(
               nameof(SelecteColorChanged),
               RoutingStrategy.Direct,
               typeof(EventHandler<RoutedEventArgs>),
               typeof(LiteColorPicker));

        public event RoutedEventHandler SelecteColorChanged
        {
            add { AddHandler(SelecteColorChangedRoutedEvent, value); }
            remove { RemoveHandler(SelecteColorChangedRoutedEvent, value); }
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set
            {
                Color oldColor = (Color)GetValue(SelectedColorProperty);

                if (oldColor == value) return;

                SetValue(SelectedColorProperty, value);
                RaiseEvent(new RoutedEventArgs(SelecteColorChangedRoutedEvent));

                if (_updateColor) __color_picker.SelectedColor = value;
            }
        }

        public LiteColorPicker()
        {
            InitializeComponent();
        }

        private void OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                __main_border.Background = new SolidColorBrush(e.NewValue.Value);
                __text.Foreground = e.NewValue.Value.GetBrushReverse();
                __main_border.BorderBrush = __text.Foreground.Clone();
                __text.Text = __main_border.Background.ToHex();

                if (SelectedColor != e.NewValue.Value)
                {
                    _updateColor = false;
                    SelectedColor = e.NewValue.Value;
                    _updateColor = true;
                }

                __text.FontSize = MeasureHelper.GetBestFontSize(__text.Text, __main_border.ActualWidth, __main_border.ActualHeight, 0.8);
            }
        }
    }

    public partial class LiteColorPicker
    {
        private bool _updateColor = true;
        private bool _clicked = false;

        private void OnLiteColorPickerLoaded(object sender, RoutedEventArgs e)
        {
            __color_picker.SelectedColor = SelectedColor;
            RaiseEvent(new RoutedEventArgs(SelecteColorChangedRoutedEvent));
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            __border.BorderBrush.Opacity = 1;

            base.OnMouseLeave(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            __border.BorderBrush.Opacity = 0.6;

            base.OnMouseEnter(e);
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
