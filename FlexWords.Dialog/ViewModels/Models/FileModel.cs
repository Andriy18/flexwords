using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexWords.Dialog.Extensions;

namespace FlexWords.Dialog.ViewModels.Models
{
    public sealed class FileModel : ObservableObject
    {
        private FileType _fileType;
        private ShowFolderViewModel _handler;

        public FileModel(ShowFolderViewModel handler, string path, FileType type = default)
        {
            _handler = handler;
            FullPath = path;
            FileType = type;

            Name = Path.GetFileName(path);

            if (type == FileType.Folder)
            {
                Name = Path.GetFileName(path);
                NameBackground = "#FFDA72".ToBrush();
            }
            else if (type == FileType.File)
            {
                Name = Path.GetFileNameWithoutExtension(path);
                Ext = Path.GetExtension(path).ToLower();

                if (Ext == ".txt")
                {
                    ExtBackground = "#ADADAD".ToBrush();
                }
                else if (Ext == ".epub")
                {
                    ExtBackground = "#8ABA20".ToBrush();
                }
                else if (Ext == ".pdf")
                {
                    ExtBackground = "#B90010".ToBrush();
                }
                else if (Ext == ".docx")
                {
                    ExtBackground = "#185ABD".ToBrush();
                }
            }
            else
            {
                Name = path;
                NameBackground = "#E1E3E6".ToBrush();
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
