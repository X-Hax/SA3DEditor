using SA3D.Texturing;
using SA3DEditor.ViewModel;

namespace SA3DEditor.ViewModel.TreeItems
{
    public class VmTexture : VmTreeItem
    {
        public Texture Texture { get; set; }

        public override TreeItemType ItemType
            => TreeItemType.Texture;

        public override string ItemName
            => Texture.Name;

        public VmTexture(VmMain mainViewModel, VmTreeItem? parent, Texture texture)
            : base(mainViewModel, parent)
        {
            Texture = texture;
        }

    }
}
