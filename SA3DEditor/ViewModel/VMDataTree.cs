using SA3D.Common.WPF.MVVM;
using SA3DEditor.ViewModel.TreeItems;
using SA3DEditor.Rendering;
using System.Collections.ObjectModel;

namespace SA3DEditor.ViewModel
{
    public class VmDataTree : BaseViewModel
    {
        private readonly VmMain _mainViewModel;

        private VmTreeItem? _selected;

        public ObservableCollection<VmTreeItem> Items { get; }

        public VmTreeItem? Selected
        {
            get => _selected;
            set
            {
                if(_selected == value)
                {
                    return;
                }

                _selected = value;

                _selected?.Select();
                OnPropertyChanged(nameof(Selected));
            }
        }

        public VmDataTree(VmMain mainVM)
        {
            _mainViewModel = mainVM;

            Items = new();

            foreach(RenderTask task in _mainViewModel.Environment.Tasks.Values)
            {
                Items.Add(new VmTask(_mainViewModel, null, task, null));
            }
        }

        public void Reset()
        {
            Selected = null;
            Items.Clear();
        }
    }
}
