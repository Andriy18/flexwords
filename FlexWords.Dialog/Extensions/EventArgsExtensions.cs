using System.Windows.Input;
using System.Windows;

namespace FlexWords.Dialog.Extensions
{
    public static class EventArgsExtensions
    {
        public static bool TryCheckNullArgs<T>(this RoutedEventArgs? args, out T obj) where T : class
        {
#pragma warning disable CS8625
            obj = default;
#pragma warning restore CS8625

            if (args is null || args.Source is not T instance) return false;

            obj = instance;

            return true;
        }

        public static bool TryCheckButtonNullArgs<T>(this MouseButtonEventArgs? args, out T obj) where T : class
        {
#pragma warning disable CS8625
            obj = default;
#pragma warning restore CS8625

            if (args is null || args.Source is not T instance || args.ChangedButton is not MouseButton.Left) return false;

            obj = instance;

            return true;
        }

        public static bool TryCheckRightButtonNullArgs<T>(this MouseButtonEventArgs? args, out T obj) where T : class
        {
#pragma warning disable CS8625
            obj = default;
#pragma warning restore CS8625

            if (args is null || args.Source is not T instance || args.ChangedButton is not MouseButton.Right) return false;

            obj = instance;

            return true;
        }
    }
}
