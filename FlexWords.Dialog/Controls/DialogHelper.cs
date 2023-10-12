﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Threading;
using FlexWords.Dialog.Extensions;
using System.Reflection;
using System.Diagnostics;
using FlexWords.Dialog.Helpers;

namespace FlexWords.Dialog.Controls
{
    public partial class FlexWordsDialog
    {
        public static DpiScale scale;

        private void ApplyLogoLoading()
        {
            Dispatcher.BeginInvoke(async () =>
            {
                if (Options.LogoPreview)
                {
                    await Task.Delay(1000);

                    var animation1 = new DoubleAnimation(-0.2, TimeSpan.FromSeconds(3.0));
                    var animation2 = new DoubleAnimation(-0.1, TimeSpan.FromSeconds(3.0));
                    var animation3 = new DoubleAnimation(0.0, TimeSpan.FromSeconds(3.0));

                    __stop_1.BeginAnimation(GradientStop.OffsetProperty, animation1);
                    __stop_2.BeginAnimation(GradientStop.OffsetProperty, animation2);
                    __stop_3.BeginAnimation(GradientStop.OffsetProperty, animation3);

                    await Task.Delay(3500);
                    Brush brush = "#E5E5E5".ToBrush();
                    brush.Opacity = 0.0;
                    __temp_logo_path.Foreground = brush;
                    var animation4 = new DoubleAnimation(1.0, TimeSpan.FromSeconds(1.5));
                    animation4.EasingFunction = new BounceEase { EasingMode = EasingMode.EaseIn };
                    __temp_logo_path.Foreground.BeginAnimation(Brush.OpacityProperty, animation4);

                    await Task.Delay(200);
                    __temp_logo_name.BeginAnimation(TextBlock.OpacityProperty, animation4);

                    await Task.Delay(2000);
                }

                var animation = new DoubleAnimation(0, TimeSpan.FromSeconds(1.5));
                animation.EasingFunction = new PowerEase { EasingMode = EasingMode.EaseIn };
                __temp_logo_border.BeginAnimation(UIElement.OpacityProperty, animation);
                await Task.Delay(1500);
                __grid.Children.Remove(__temp_logo_border);
            }, DispatcherPriority.Send);
        }

        public void InitializeDpi()
        {
            scale = VisualTreeHelper.GetDpi(this);
        }

        public static Workspace GetCurrentWorkspace()
        {
            if (Options.LastWorkspaceIndex == 0) return Options.Workspace1;
            else if (Options.LastWorkspaceIndex == 1) return Options.Workspace2;

            return Options.Workspace1;
        }

        public static void SaveWorkspace(Workspace workspace)
        {
            if (Options.LastWorkspaceIndex == 0) Options.Workspace1 = workspace;
            else if (Options.LastWorkspaceIndex == 1) Options.Workspace2 = workspace;
        }

        public static string? CurrentVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fileVersionInfo.ProductVersion;
            }
        }

        public static double GetCurrentFontHeight()
        {
            string name = FontFamilyHelper.GetFontFamilyName(Options.FontFamily);

            return MeasureHelper.GetFontHeight(Options.FontSize, name);
        }
    }
}
