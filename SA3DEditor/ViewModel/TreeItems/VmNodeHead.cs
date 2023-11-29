using SA3D.Modeling.ObjectData;
using SA3DEditor.ViewModel;
using System.Collections.Generic;

namespace SA3DEditor.ViewModel.TreeItems
{
    public class VmNodeHead : VmTreeItem
    {
        public Node ObjectData { get; }

        public override TreeItemType ItemType
            => TreeItemType.NodeHead;

        public override string ItemName
            => "Model";

        public override bool Expandable
            => true;

        protected override List<VmTreeItem> Expand()
        {
            return new() { new VmNode(MainViewModel, this, ObjectData) };
        }


        public VmNodeHead(VmMain mainViewModel, VmTreeItem? parent, Node objectData)
            : base(mainViewModel, parent, true)
        {
            ObjectData = objectData;
        }
    }
}
