using SA3D.Modeling.ObjectData;
using System.Collections.Generic;
using System.Numerics;

namespace SA3DEditor.ViewModel.TreeItems
{
    public class VmNode : VmTreeItem
    {
        public Node NodeData { get; }

        #region Transform wrappers

        public float PositionX
        {
            get => NodeData.Position.X;
            set
            {
                Vector3 v = NodeData.Position;
                v.X = value;
                NodeData.Position = v;
            }
        }

        public float PositionY
        {
            get => NodeData.Position.Y;
            set
            {
                Vector3 v = NodeData.Position;
                v.Y = value;
                NodeData.Position = v;
            }
        }

        public float PositionZ
        {
            get => NodeData.Position.Z;
            set
            {
                Vector3 v = NodeData.Position;
                v.Z = value;
                NodeData.Position = v;
            }
        }


        public float RotationX
        {
            get => NodeData.EulerRotation.X;
            set
            {
                Vector3 v = NodeData.EulerRotation;
                v.X = value;
                NodeData.EulerRotation = v;
            }
        }

        public float RotationY
        {
            get => NodeData.EulerRotation.Y;
            set
            {
                Vector3 v = NodeData.EulerRotation;
                v.Y = value;
                NodeData.EulerRotation = v;
            }
        }

        public float RotationZ
        {
            get => NodeData.EulerRotation.Z;
            set
            {
                Vector3 v = NodeData.EulerRotation;
                v.Z = value;
                NodeData.EulerRotation = v;
            }
        }


        public float ScaleX
        {
            get => NodeData.Scale.X;
            set
            {
                Vector3 v = NodeData.Scale;
                v.X = value;
                NodeData.Scale = v;
            }
        }

        public float ScaleY
        {
            get => NodeData.Scale.Y;
            set
            {
                Vector3 v = NodeData.Scale;
                v.Y = value;
                NodeData.Scale = v;
            }
        }

        public float ScaleZ
        {
            get => NodeData.Scale.Z;
            set
            {
                Vector3 v = NodeData.Scale;
                v.Z = value;
                NodeData.Scale = v;
            }
        }

        #endregion

        public override TreeItemType ItemType
            => TreeItemType.Node;

        public override string ItemName
            => NodeData.Label;

        public override bool Expandable
            => NodeData.ChildCount > 0;

        protected override List<VmTreeItem> Expand()
        {
            List<VmTreeItem> result = new();
            for(int i = 0; i < NodeData.ChildCount; i++)
            {
                result.Add(new VmNode(MainViewModel, this, NodeData[i]));
            }

            return result;
        }

        public override void Select()
        {
            MainViewModel.Environment.ActiveNode = NodeData;
        }

        public VmNode(VmMain mainViewModel, VmTreeItem? parent, Node node)
            : base(mainViewModel, parent, node.ChildCount > 0)
        {
            NodeData = node;
        }
    }
}
