using SA3DEditor.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace SA3DEditor.XAML.UserControls
{
    /// <summary>
    /// Interaction logic for UCSATreeTab.xaml
    /// </summary>
    public partial class UcSATreeTab : UserControl
    {
        public UcSATreeTab()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ((VmDataTree)DataContext).Selected = (VmTreeItem)e.NewValue;
        }
    }
}
