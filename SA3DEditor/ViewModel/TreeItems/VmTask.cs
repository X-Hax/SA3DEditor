using SA3D.Modeling.ObjectData;
using SA3DEditor.ViewModel.TreeItems;
using SA3DEditor.Rendering;
using System.Collections.Generic;
using SA3D.Modeling.File;

namespace SA3DEditor.ViewModel.TreeItems
{
    public class VmTask : VmTreeItem
    {
        public override TreeItemType ItemType
            => TreeItemType.Task;

        public override string ItemName
            => Task.Name;

        public override bool Expandable
            => true;

        public RenderTask Task { get; }

        public ModelFile? ModelFileInfo { get; }

        protected override List<VmTreeItem> Expand()
        {
            return new()
            {
                new VmNodeHead(MainViewModel, this, Task.Model),
                new VmTextureHead(MainViewModel, this, Task.Textures),
                new VmAnimHead(MainViewModel, this, Task.Animations)
            };
        }

        public VmTask(VmMain mainViewModel, VmTreeItem? parent, RenderTask taskData, ModelFile? modelFileInfo)
            : base(mainViewModel, parent, true)
        {
            Task = taskData;
            ModelFileInfo = modelFileInfo;
        }
    }
}
