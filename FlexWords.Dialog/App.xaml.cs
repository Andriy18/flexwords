using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace FlexWords.Dialog
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnAppUnhandledException;
        }

        private void OnAppUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string filter = "JSON file (*.json)|*.json";
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var ofd = new SaveFileDialog
            {
                Filter = filter,
                InitialDirectory = folder
            };

            if (ofd.ShowDialog() ?? false)
            {
                Exception exception = e.Exception;
                File.WriteAllText(ofd.FileName, JsonConvert.SerializeObject(exception));
            }

            e.Handled = true;
        }
    }
}
