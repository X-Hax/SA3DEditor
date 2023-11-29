using SA3D.Common.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace SA3DEditor.ViewModel
{
    public enum TreeItemType
    {
        Task,
        NodeHead,
        Node,
        AnimationHead,
        Animation,

        VisualHead,
        Visual,
        CollisionHead,
        Collision,
        AttachHead,
        Attach,

        TextureHead,
        Texture
    }

    [ValueConversion(typeof(TreeItemType), typeof(string))]
    public class TreeItemTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "/SA3D;component/Icons/TreeIcons/" + (TreeItemType)value switch
            {
                TreeItemType.NodeHead or TreeItemType.Node => "Model.png",
                TreeItemType.AnimationHead or TreeItemType.Animation => "Animation.png",
                TreeItemType.TextureHead => "Textures.png",
                TreeItemType.Texture => "Texture.png",
                _ => "Object.png",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public abstract class VmTreeItem : BaseViewModel
    {
        private bool _expanded;
        private bool _loaded;

        protected VmMain MainViewModel { get; }

        public VmTreeItem? Parent { get; }

        public ObservableCollection<VmTreeItem?> Children { get; private set; }

        public abstract TreeItemType ItemType { get; }

        public abstract string ItemName { get; }

        public virtual bool Expandable => false;

        public bool IsExpanded
        {
            get => _expanded;
            set
            {
                _expanded = value;
                if(value && !_loaded)
                {
                    SetupChildren();
                    _loaded = true;
                }
            }
        }

        public VmTreeItem(VmMain mainViewModel, VmTreeItem? parent, bool expandable = false)
        {
            MainViewModel = mainViewModel;
            Parent = parent;
            Children = new();
            if(expandable)
            {
                Children.Add(null);
            }
        }


        public void Refresh()
        {
            if(_loaded)
            {
                SetupChildren();
            }
        }

        private void SetupChildren()
        {
            Children.Clear();
            foreach(VmTreeItem child in Expand())
            {
                Children.Add(child);
            }
        }

        public T? TryGetParent<T>() where T : VmTreeItem
        {
            VmTreeItem? item = Parent;

            while(item != null)
            {
                if(item is T result)
                {
                    return result;
                }

                item = item.Parent;
            }

            return null;
        }

        protected virtual List<VmTreeItem> Expand()
            => new();

        public virtual void Select() { }

    }

}