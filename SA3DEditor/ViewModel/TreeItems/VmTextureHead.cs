using SA3D.Texturing;
using SA3DEditor.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace SA3DEditor.ViewModel.TreeItems
{
    public class VmTextureHead : VmTreeItem
    {
        public TextureSet? Textures { get; set; }

        public override TreeItemType ItemType
            => TreeItemType.TextureHead;

        public override string ItemName
            => "Textures";

        public override bool Expandable
            => Textures?.Textures.Count > 0;

        protected override List<VmTreeItem> Expand()
        {
            if(Textures == null)
            {
                return new();
            }

            return Textures.Textures.Select<Texture, VmTreeItem>(x => new VmTexture(MainViewModel, this, x)).ToList();
        }


        public VmTextureHead(VmMain mainViewModel, VmTreeItem? parent, TextureSet? textureSet)
           : base(mainViewModel, parent, textureSet?.Textures.Count > 0)
        {
            Textures = textureSet;
        }

    }
}
