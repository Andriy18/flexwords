using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlexWords.Dialog.Controls.Components
{
    public partial class LiteCheckBox : UserControl
    {
        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.RegisterAttached(
                nameof(Checked),
                typeof(bool),
                typeof(LiteCheckBox),
                new PropertyMetadata(false));

        public static readonly RoutedEvent CheckedValueRoutedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(CheckedValueChanged),
                RoutingStrategy.Direct,
                typeof(EventHandler<RoutedEventArgs>),
                typeof(LiteCheckBox));

        public event RoutedEventHandler CheckedValueChanged
        {
            add { AddHandler(CheckedValueRoutedEvent, value); }
            remove { RemoveHandler(CheckedValueRoutedEvent, value); }
        }
        public bool Checked
        {
            get => (bool)GetValue(CheckedProperty);
            set
            {
                bool oldValue = (bool)GetValue(CheckedProperty);

                if (oldValue == value) return;

                SetValue(CheckedProperty, value);
                RaiseEvent(new RoutedEventArgs(CheckedValueRoutedEvent));

                if (_updateValue) SetCheckBoxValue(value);
            }
        }

        public LiteCheckBox()
        {
            InitializeComponent();
        }

        public void SetCheckBoxValue(bool newValue)
        {
            double newLeft = newValue ? (__internal_border.ActualWidth - __slider.ActualWidth) : 0;

            __slider.Background.Opacity = newValue ? 1.0 : 0.26;
            __slider.Margin = new Thickness(newLeft, 0, 0, 0);

            if (Checked != newValue)
            {
                _updateValue = false;
                Checked = newValue;
                _updateValue = true;
            }
        }
    }

    public partial class LiteCheckBox
    {
        private bool _clicked = false;
        private bool _updateValue = true;

        private void OnLiteCheckBoxLoaded(object sender, RoutedEventArgs e)
        {
            SetCheckBoxValue(Checked);
            RaiseEvent(new RoutedEventArgs(CheckedValueRoutedEvent));
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

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_clicked)
            {
                SetCheckBoxValue(!Checked);
                _clicked = false;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _clicked = true;

            base.OnMouseDown(e);
        }

        protected sealed override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            double cornerRadius = sizeInfo.NewSize.Height / 2;
            double smallCornerRadius = cornerRadius - 3;

            __slider.Width = sizeInfo.NewSize.Width / 2 + 4;
            __border.CornerRadius = new CornerRadius(cornerRadius);
            __internal_border.CornerRadius = new CornerRadius(smallCornerRadius);
            __slider.CornerRadius = new CornerRadius(smallCornerRadius);
            __slider.Height = smallCornerRadius * 2;

            base.OnRenderSizeChanged(sizeInfo);
        }
    }
}
