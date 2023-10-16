using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlexWords.Dialog.Controls.Components
{
    public partial class LiteCheckBox : UserControl
    {
        private bool _clicked = false;

        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.Register(
                nameof(Checked),
                typeof(bool),
                typeof(LiteCheckBox),
                new FrameworkPropertyMetadata(default(bool),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool Checked
        {
            get => (bool)GetValue(CheckedProperty);
            set
            {
                bool oldValue = (bool)GetValue(CheckedProperty);
                bool newValue = value;

                if (oldValue == newValue) return;

                SetValue(CheckedProperty, value);
            }
        }

        public LiteCheckBox()
        {
            InitializeComponent();
        }

        private void UpdateCheckBox(bool newValue)
        {
            double newLeft = newValue ? (__internal_border.ActualWidth - __slider.ActualWidth) : 0;

            __slider.Background.Opacity = newValue ? 1.0 : 0.26;
            __slider.Margin = new Thickness(newLeft, 0, 0, 0);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == CheckedProperty)
            {
                UpdateCheckBox((bool)e.NewValue);
            }

            base.OnPropertyChanged(e);
        }

        private void OnLiteCheckBoxLoaded(object sender, RoutedEventArgs e)
        {
            UpdateCheckBox(Checked);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_clicked)
            {
                Checked = !Checked;
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
