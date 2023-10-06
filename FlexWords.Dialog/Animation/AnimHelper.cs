using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace FlexWords.Dialog.Animation
{
    public static class AnimHelper
    {
        public readonly static TimeSpan TS50 = TimeSpan.FromSeconds(0.5);
        public readonly static TimeSpan TS25 = TimeSpan.FromSeconds(0.25);
        public readonly static TimeSpan TS20 = TimeSpan.FromSeconds(0.2);
        public readonly static TimeSpan TS15 = TimeSpan.FromSeconds(0.15);
        public readonly static TimeSpan TS13 = TimeSpan.FromSeconds(0.13);
        public readonly static TimeSpan TS10 = TimeSpan.FromSeconds(0.1);
        public readonly static TimeSpan TS05 = TimeSpan.FromSeconds(0.05);

        public static Action TextHoverTo(TextBlock text, Brush toBrush)
        {
            Brush baseBrush = text.Foreground.Clone();
            Brush brush = toBrush.Clone();

            text.MouseEnter += OnTextHoverEnter;
            text.MouseLeave += OnTextHoverLeave;

            void OnTextHoverEnter(object sender, RoutedEventArgs e)
            {
                var element = (TextBlock)sender;
                element.Foreground = brush;
            }

            void OnTextHoverLeave(object sender, RoutedEventArgs e)
            {
                var element = (TextBlock)sender;
                element.Foreground = baseBrush;
            }

            return () =>
            {
                text.MouseEnter -= OnTextHoverEnter;
                text.MouseLeave -= OnTextHoverLeave;
            };
        }

        public static Action GeometryOpacity(UIElement hoverBoundsElement, Shape shape)
        {
            hoverBoundsElement.MouseEnter += OnMouseEnter;
            hoverBoundsElement.MouseLeave += OnMouseLeave;
            
            void OnMouseEnter(object sender, MouseEventArgs e)
            {
                var element = (UIElement)sender;
                shape.Opacity = 0.62;
            }

            void OnMouseLeave(object sender, MouseEventArgs e)
            {
                var element = (UIElement)sender;
                shape.Opacity = 1;
            }

            return () =>
            {
                hoverBoundsElement.MouseEnter -= OnMouseEnter;
                hoverBoundsElement.MouseLeave -= OnMouseLeave;
            };
        }

        public static void RenderTransformMove(FrameworkElement element, double x, double y)
        {
            var animationX = new DoubleAnimation(x, TS20);
            var animationY = new DoubleAnimation(y, TS20);

            element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animationX);
            element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, animationY);
        }

        public static void RenderTransformMove(FrameworkElement margin, double value, bool moveX = false)
        {
            RenderTransformMove(margin, moveX ? value : 0, moveX ? 0 : value);
        }

        public static void DoubleFlashingTextBlock(TextBlock textBlock)
        {
            textBlock.Dispatcher.BeginInvoke(async (TextBlock block) =>
            {
                block.Foreground = block.Foreground.Clone();
                var animation = new DoubleAnimation(0, TS15);
                block.Foreground.BeginAnimation(Brush.OpacityProperty, animation);

                await Task.Delay(150);

                animation = new DoubleAnimation(1, TS05);
                block.Foreground.BeginAnimation(Brush.OpacityProperty, animation);
            }, System.Windows.Threading.DispatcherPriority.Send, textBlock);
        }

        internal static void Flickering(UIElement element, bool stopping = false)
        {
            if (stopping)
            {
                element.BeginAnimation(UIElement.OpacityProperty, null);
                element.Opacity = 1;

                return;
            }

            var animation = new DoubleAnimation(1, 0.2, TimeSpan.FromSeconds(0.6))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            element.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public static void DoubleFlashing(UIElement element)
        {
            element.Dispatcher.BeginInvoke(async (UIElement obj) =>
            {
                var animation = new DoubleAnimation(0, TS15);
                obj.BeginAnimation(Brush.OpacityProperty, animation);

                await Task.Delay(150);

                animation = new DoubleAnimation(1, TS05);
                obj.BeginAnimation(Brush.OpacityProperty, animation);
            }, System.Windows.Threading.DispatcherPriority.Send, element);
        }
    }
}
