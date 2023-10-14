using FlexWords.Dialog.ViewModels;
using System.Windows.Controls;

namespace FlexWords.Dialog.Controls.Components
{
    public partial class ShowFolder : UserControl
    {
        public ShowFolder()
        {
            InitializeComponent();
            DataContext = new ShowFolderViewModel();
        }
    }
}
