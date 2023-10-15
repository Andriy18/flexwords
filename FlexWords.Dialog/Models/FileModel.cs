using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexWords.Constants;
using FlexWords.Dialog.Extensions;
using FlexWords.Dialog.ViewModels;
using FlexWords.Entities.Enums;

namespace FlexWords.Dialog.Models
{
    public sealed class FileModel : ObservableObject
    {
        private readonly ShowFolderViewModel _handler;
        private FileType _fileType;

        public FileModel(ShowFolderViewModel handler, string path, FileType type = default)
        {
            _handler = handler;
            FullPath = path;
            FileType = type;

            Name = Path.GetFileName(path);

            if (type is FileType.Folder)
            {
                Name = Path.GetFileName(path);
                NameBackground = Words.FileFormat.FormatThemes[type.ToString()].ToBrush();
            }
            else if (type is FileType.File)
            {
                Name = Path.GetFileNameWithoutExtension(path);
                Ext = Path.GetExtension(path).ToLower();
                ExtBackground = Words.FileFormat.FormatThemes[Ext].ToBrush();
            }
            else if (type is FileType.Driver)
            {
                Name = path;
                NameBackground = Words.FileFormat.FormatThemes[type.ToString()].ToBrush();
            }

            OnFileClickedCommand = new RelayCommand<MouseButtonEventArgs>(OnFileClicked);
        }

        public IRelayCommand OnFileClickedCommand { get; }


        public FileType FileType
        {
            get => _fileType;
            set
            {
                _fileType = value;

                IsFile = _fileType == FileType.File;
                IsFolder = _fileType == FileType.Folder;
                IsDriver = _fileType == FileType.Driver;

                OnPropertyChanged(nameof(IsFile));
                OnPropertyChanged(nameof(IsFolder));
                OnPropertyChanged(nameof(IsDriver));
            }
        }
        public string FullPath { get; set; }
        public bool IsFile { get; set; }
        public bool IsFolder { get; set; }
        public bool IsDriver { get; set; }
        public string? Name { get; set; }
        public string? Ext { get; set; }
        public Brush? NameBackground { get; set; }
        public Brush? ExtBackground { get; set; }

        private void OnFileClicked(MouseButtonEventArgs? args)
        {
            if (!args.TryCheckButtonNullArgs(out Border _)) return;

            if (IsFile)
            {
                App.Dialog.OpenBook(FullPath);
            }
            else _handler.UpdateContent(FullPath);
        }
    }
}
