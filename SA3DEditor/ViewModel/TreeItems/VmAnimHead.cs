using SA3D.Modeling.Animation;
using SA3DEditor.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace SA3DEditor.ViewModel.TreeItems
{
    public class VmAnimHead : VmTreeItem
    {
        public List<RenderMotion> Animations { get; }

        public override TreeItemType ItemType
            => TreeItemType.AnimationHead;

        public override string ItemName
            => "Animations";

        public override bool Expandable
            => Animations.Count > 0;

        protected override List<VmTreeItem> Expand()
        {
            return Animations.Select<RenderMotion, VmTreeItem>(x => new VmAnimation(MainViewModel, this, x)).ToList();
        }

        public VmAnimHead(VmMain mainViewModel, VmTreeItem? parent, List<RenderMotion> animations)
            : base(mainViewModel, parent, animations.Count > 0)
        {
            Animations = animations;
        }
    }
}
