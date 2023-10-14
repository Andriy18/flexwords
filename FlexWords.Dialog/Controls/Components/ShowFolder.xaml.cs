using FlexWords.Dialog.ViewModels;
using System.Windows.Controls;

namespace FlexWords.Dialog.Controls.Components
{
    public partial class ShowFolder : UserControl
    {
        public ShowFolderViewModel ViewModel { get; }

        public ShowFolder()
        {
            InitializeComponent();
            ViewModel = new ShowFolderViewModel(this);
            DataContext = ViewModel;
        }
    }
}
