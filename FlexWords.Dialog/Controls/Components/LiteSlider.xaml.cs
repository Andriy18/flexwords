using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Media;
using FlexWords.Dialog.Helpers;

namespace FlexWords.Dialog.Controls.Components
{
    public partial class LiteSlider : UserControl
    {
        public static readonly DependencyProperty UseIntegerProperty =
            DependencyProperty.RegisterAttached(
                nameof(UseInteger),
                typeof(bool),
                typeof(LiteSlider),
                new PropertyMetadata(true));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached(
                nameof(Value),
                typeof(double),
                typeof(LiteSlider),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.RegisterAttached(
                nameof(MaxValue),
                typeof(double),
                typeof(LiteSlider),
                new PropertyMetadata(100.0));

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.RegisterAttached(
                nameof(MinValue),
                typeof(double),
                typeof(LiteSlider),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.RegisterAttached(
                nameof(Step),
                typeof(double?),
                typeof(LiteSlider),
                new PropertyMetadata(null));

        public static readonly RoutedEvent ValueChangedRoutedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ValueChanged),
                RoutingStrategy.Direct,
                typeof(EventHandler<RoutedEventArgs>),
                typeof(LiteSlider));

        public static readonly DependencyProperty AnimateHoverProperty =
           DependencyProperty.RegisterAttached(
               nameof(AnimateHover),
               typeof(bool),
               typeof(LiteSlider),
               new PropertyMetadata(true));

        public static readonly DependencyProperty LazyLoadingProperty =
           DependencyProperty.RegisterAttached(
               nameof(LazyLoading),
               typeof(bool),
               typeof(LiteSlider),
               new PropertyMetadata(false));

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedRoutedEvent, value); }
            remove { RemoveHandler(ValueChangedRoutedEvent, value); }
        }
        public bool UseInteger
        {
            get => (bool)GetValue(UseIntegerProperty);
            set => SetValue(UseIntegerProperty, value);
        }
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set
            {
                double oldValue = (double)GetValue(ValueProperty);
                double newValue = ClampValue(value);

                if (Math.Abs(newValue - oldValue) < double.Epsilon) return;

                SetValue(ValueProperty, newValue);

                if (_updateSlider)
                {
                    double newWidth = GetWidthRelativeToValue(newValue);
                    newWidth = ClampWidth(newWidth);
                    __slider.Margin = new Thickness(newWidth, 0, 0, 0);
                }
                
                if (_canRaiseEvent) RaiseEvent(new RoutedEventArgs(ValueChangedRoutedEvent));

                UpdateText();
            }
        }
        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }
        public double? Step
        {
            get => (double?)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }
        public bool AnimateHover
        {
            get => (bool)GetValue(AnimateHoverProperty);
            set => SetValue(AnimateHoverProperty, value);
        }
        public bool LazyLoading
        {
            get => (bool)GetValue(LazyLoadingProperty);
            set => SetValue(LazyLoadingProperty, value);
        }

        public LiteSlider()
        {
            InitializeComponent();

            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                double newX = GetMousePosition().X;
                double newWidth = ((newX - _mouseX) / _dpiX) + __slider.Margin.Left;
                _mouseX = newX;

                newWidth = ClampWidth(newWidth);
                __slider.Margin = new Thickness(newWidth, 0, 0, 0);

                double newVlaue = GetValueRelativeToWidth(newWidth);
                newVlaue = ClampValue(newVlaue);

                if (Math.Abs(newVlaue - Value) < double.Epsilon) return;
                {
                    _updateSlider = false;
                    Value = newVlaue;
                    _updateSlider = true;
                }
            }, System.Windows.Threading.DispatcherPriority.Send);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0) Value += _scrollStep;
            else if (e.Delta < 0) Value -= _scrollStep;

            base.OnMouseWheel(e);
        }

        private double _scrollStep
        {
            get
            {
                if (Step is null) return UseInteger ? 1.0 : 0.01;

                return UseInteger ? Math.Round(Step.Value, MidpointRounding.AwayFromZero) : Step.Value;
            }
        }
        private double _maxSliderWidth => __internal_border.ActualWidth - __slider.ActualWidth;

        private double GetValueRelativeToWidth(double width)
        {
            return (width / (_maxSliderWidth / (MaxValue - MinValue))) + MinValue;
        }

        private double GetWidthRelativeToValue(double value)
        {
            return (value - MinValue) * (_maxSliderWidth / (MaxValue - MinValue));
        }

        private double ClampWidth(double width)
        {
            if (double.IsNaN(width))
            {
                width = 0;
            }

            return Math.Clamp(width, 0, _maxSliderWidth);
        }

        private double ClampValue(double value)
        {
            value = Math.Clamp(value, MinValue, MaxValue);

            if (UseInteger) value = Math.Round(value, MidpointRounding.AwayFromZero);

            if (Step is not null)
            {
                value = Step.Value * Math.Round(value / Step.Value, MidpointRounding.AwayFromZero);
            }

            return value;
        }
    }

    public partial class LiteSlider
    {
        private readonly Timer _timer = new(15);
        private double _mouseX;
        private double _dpiX;
        private bool _canRaiseEvent = true;
        private bool _updateSlider = true;

        private void OnLiteSliderLoaded(object sender, RoutedEventArgs e)
        {
            _dpiX = VisualTreeHelper.GetDpi(this).DpiScaleX;

            double newWidth = GetWidthRelativeToValue(Value);
            newWidth = ClampWidth(newWidth);
            __slider.Margin = new Thickness(newWidth, 0, 0, 0);
            RaiseEvent(new RoutedEventArgs(ValueChangedRoutedEvent));
            UpdateText();
        }

        private void UpdateText()
        {
            if (UseInteger)
            {
                __text.Text = $"{Math.Round(Value, MidpointRounding.ToEven)}";
            }
            else
            {
                float value = (float)Math.Round(Value, 2, MidpointRounding.ToEven);

                if (value % 1.0 > float.Epsilon) __text.Text = $"{value}";
                else __text.Text = $"{value:f1}";
            }

            __text.FontSize = MeasureHelper.GetBestFontSize(__text.Text, __slider.ActualWidth, __slider.ActualHeight, 0.8);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            SetBorderBrushOpacity(1);

            base.OnMouseLeave(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            SetBorderBrushOpacity(0.6);

            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _mouseX = GetMousePosition().X;

            if (!_timer.Enabled)
            {
                if (LazyLoading) _canRaiseEvent = false;

                _timer.Start();
            }

            SetSliderBackgroundOpacity(0.8);

            this.CaptureMouse();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_timer.Enabled)
            {
                _timer.Stop();

                _canRaiseEvent = true;
                if (LazyLoading) RaiseEvent(new RoutedEventArgs(ValueChangedRoutedEvent));
            }

            SetSliderBackgroundOpacity(1);

            this.ReleaseMouseCapture();
            base.OnMouseUp(e);
        }

        private void SetSliderBackgroundOpacity(double opacity)
        {
            __slider.Background.Opacity = opacity;
        }

        private void SetBorderBrushOpacity(double opacity)
        {
            if (AnimateHover) __border.BorderBrush.Opacity = opacity;
        }

        protected sealed override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            double cornerRadius = sizeInfo.NewSize.Height / 2;
            double smallCornerRadius = cornerRadius - 3;

            __border.CornerRadius = new CornerRadius(cornerRadius);
            __internal_border.CornerRadius = new CornerRadius(smallCornerRadius);
            __slider.CornerRadius = new CornerRadius(smallCornerRadius);
            __slider.Height = smallCornerRadius * 2.0;
            __slider.Width = smallCornerRadius * 4.8;

            double newWidth = GetWidthRelativeToValue(Value);
            newWidth = ClampWidth(newWidth);
            __slider.Margin = new Thickness(newWidth, 0, 0, 0);

            base.OnRenderSizeChanged(sizeInfo);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }
    }
}
