using SA3D.Modeling.ObjectData;
using SA3DEditor.ViewModel.TreeItems;
using System.Collections.Generic;
using System.Linq;

namespace SA3DEditor.ViewModel.TreeItems
{
    public class VmLandEntryHead : VmTreeItem
    {
        public LandEntry[] Geometry { get; set; }

        public bool IsCollision { get; }

        public override TreeItemType ItemType
            => IsCollision ? TreeItemType.CollisionHead : TreeItemType.VisualHead;

        public override string ItemName
            => IsCollision ? "Collision Geometry" : "Visual Geometry";

        public override bool Expandable
            => Geometry.Length > 0;

        protected override List<VmTreeItem> Expand()
        {
            return Geometry.Select<LandEntry, VmTreeItem>(x => new VmLandEntry(MainViewModel, this, x, IsCollision)).ToList();
        }

        public VmLandEntryHead(VmMain mainViewModel, VmTreeItem? parent, LandEntry[] geometry, bool collision)
            : base(mainViewModel, parent, geometry.Length > 0)
        {
            IsCollision = collision;
            Geometry = geometry;
        }
    }
}
