using SA3D.Modeling.ObjectData;

namespace SA3DEditor.ViewModel.TreeItems
{
    public class VmLandEntry : VmTreeItem
    {
        public LandEntry LandEntry { get; }

        public bool IsCollision { get; }

        public override TreeItemType ItemType
            => IsCollision ? TreeItemType.Collision : TreeItemType.Visual;

        public override string ItemName
            => LandEntry.Model.Label;

        public override void Select()
        {
            MainViewModel.Environment.ActiveLandEntry = LandEntry;
        }

        public VmLandEntry(VmMain mainViewModel, VmTreeItem? parent, LandEntry geometry, bool isCollision)
            : base(mainViewModel, parent)
        {
            LandEntry = geometry;
            IsCollision = isCollision;
        }
    }
}
