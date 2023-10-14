using System;
using System.IO;
using System.Linq;

namespace FlexWords.Dialog.Helpers
{
    public static class FolderHelper
    {
        public static string Desktop =>
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static string[] GetFiles(string folder)
        {
            return Directory.GetFiles(folder);
        }

        public static string[] GetFolders(string folder)
        {
            return Directory.GetDirectories(folder);
        }

        public static string[] GetDrivers()
        {
            return DriveInfo.GetDrives().Select(d => d.Name).ToArray();
        }
    }
}
