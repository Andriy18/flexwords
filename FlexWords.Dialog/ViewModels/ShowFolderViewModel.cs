using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexWords.Constants;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.Helpers;
using FlexWords.Dialog.Models;
using FlexWords.Entities.Enums;

namespace FlexWords.Dialog.ViewModels
{
    public class ShowFolderViewModel : ObservableObject
    {
        private string? _folder;

        public string? Folder
        {
            get => _folder;
            set
            {
                _folder = value;
                Options.LastUsedFolder = _folder ?? string.Empty;
            }
        }
        public string? FolderName { get; set; }

        public ShowFolderViewModel()
        {
            Items = new ObservableCollection<FileModel>();
            OnBackClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnBackClicked);

            Folder = Options.LastUsedFolder;
            InitializeFolder();
        }

        public ObservableCollection<FileModel> Items { get; set; }

        public IRelayCommand OnBackClickedCommand { get;}

        private void InitializeFolder()
        {
            if (string.IsNullOrEmpty(Folder)) UpdateContentDriver();
            else UpdateContent(Folder);
        }

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

                if (Words.FileFormat.SupportedFormats.Contains(ext))
                {
                    Items.Add(new FileModel(this, file));
                }
            }

            OnPropertyChanged(nameof(Items));
        }

        public void UpdateContentDriver()
        {
            Folder = null;
            FolderName = null;

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
            if (!args.TryCheckButtonNullArgs(out Border _) || Folder is null) return;

            DirectoryInfo? parent = Directory.GetParent(Folder);

            if (parent is null) UpdateContentDriver();
            else UpdateContent(parent.FullName);
        }
    }
}
