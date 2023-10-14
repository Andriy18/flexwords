using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexWords.Dialog.Controls.Components;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.Helpers;
using FlexWords.Dialog.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlexWords.Dialog.ViewModels
{
    public class ShowFolderViewModel : ObservableObject
    {
        public static string[] SupportedExtensions =
        {
            ".txt",
            ".epub",
            ".pdf",
            ".docx",
        };

        private ShowFolder _control;
        public string? Folder { get; set; }
        public string? FolderName { get; set; }

        public ShowFolderViewModel(ShowFolder control)
        {
            _control = control;
            Items = new ObservableCollection<FileModel>();
            UpdateContent(FolderHelper.Desktop);

            OnBackClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnBackClicked);
        }

        public ObservableCollection<FileModel> Items { get; set; }

        public IRelayCommand OnBackClickedCommand { get;}

        public void UpdateContent(string parent)
        {
            Folder = parent;
            FolderName = Path.GetFileName(Folder);
            if (string.IsNullOrEmpty(FolderName)) FolderName = Folder;

            OnPropertyChanged(nameof(Folder));
            OnPropertyChanged(nameof(FolderName));

            Items.Clear();

            foreach (string folder in FolderHelper.GetFolders(Folder))
            {
                Items.Add(new FileModel(this, folder, FileType.Folder));
            }

            foreach (string file in FolderHelper.GetFiles(Folder))
            {
                string ext = Path.GetExtension(file).ToLower();

                if (SupportedExtensions.Contains(ext))
                {
                    Items.Add(new FileModel(this, file));
                }
            }

            OnPropertyChanged(nameof(Items));
        }

        public void UpdateContentDriver()
        {
            Folder = string.Empty;
            FolderName = string.Empty;

            OnPropertyChanged(nameof(Folder));
            OnPropertyChanged(nameof(FolderName));

            Items.Clear();

            foreach (string folder in FolderHelper.GetDrivers())
            {
                Items.Add(new FileModel(this, folder, FileType.Driver));
            }

            OnPropertyChanged(nameof(Items));
        }

        private void OnBackClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out Border _) || string.IsNullOrEmpty(Folder)) return;

            DirectoryInfo? parent = Directory.GetParent(Folder);

            if (parent is null) UpdateContentDriver();
            else UpdateContent(parent.FullName);
        }
    }
}
