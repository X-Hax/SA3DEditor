using SA3D.Modeling.Animation;
using SA3DEditor.Rendering;

namespace SA3DEditor.ViewModel.TreeItems
{
    class VmAnimation : VmTreeItem
    {
        public RenderMotion Animation { get; }

        public override TreeItemType ItemType
            => TreeItemType.Animation;

        public override string ItemName
            => Animation.Animation.Label;

        public override void Select()
        {
            VmTask? task = TryGetParent<VmTask>();
            if(task == null)
            {
                return;
            }

            task.Task.ActiveAnimation = task.Task.Animations.IndexOf(Animation);
        }

        public VmAnimation(VmMain mainViewModel, VmTreeItem? parent, RenderMotion animation)
            : base(mainViewModel, parent)
        {
            Animation = animation;
        }
    }
}
