using SA3D.Modeling.ObjectData;
using SA3D.Modeling.Animation;
using SA3D.Texturing;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SA3DEditor.Rendering
{
    public class RenderTask
    {
        private readonly RenderEnvironment _environment;
        private TextureSet? _textures;
        private int _activeAnimation;

        public string Name { get; }

        public Node Model { get; private set; }

        public TextureSet? Textures
        {
            get => _textures;
            set
            {
                if(_textures == value)
                {
                    return;
                }

                if(_textures != null)
                {
                    _environment.Context.UnloadTextureSet(_textures);
                }

                _textures = value;

                if(_textures != null)
                {
                    _environment.Context.LoadTextureSet(_textures);
                }
            }
        }

        public List<RenderMotion> Animations { get; }

        public int ActiveAnimation
        {
            get => Animations.Count == 0 ? -1 : _activeAnimation;
            set
            {
                AnimationTimestamp = 0;
                _activeAnimation = value;
            }
        }

        public float AnimationTimestamp { get; set; }
        public float AnimationSpeed { get; set; } = 1;

        internal RenderTask(RenderEnvironment environment, Node model, string name)
        {
            _environment = environment;
            Model = model;
            Model.BufferMeshData(true);
            Name = name;
            Animations = new();
        }

        public void ReplaceModel(Node newModel)
        {
            Model = newModel;
            Model.BufferMeshData(true);
        }

        public void Update(float delta)
        {
            if(ActiveAnimation == -1)
            {
                return;
            }

            RenderMotion animation = Animations[ActiveAnimation];

            AnimationTimestamp += (float)(delta * AnimationSpeed * animation.FramesPerSecond);
            AnimationTimestamp %= animation.FrameCount - 1;

            int index = 0;
            foreach(Node node in Model.GetAnimTreeNodeEnumerable())
            {
                if(animation.Animation.Keyframes.TryGetValue(index, out Keyframes? keyframes))
                {
                    Frame frame = keyframes.GetFrameAt(AnimationTimestamp);

                    if(frame.QuaternionRotation == null)
                    {
                        node.UpdateTransforms(frame.Position, frame.EulerRotation, frame.Scale);
                    }
                    else
                    {
                        node.UpdateTransforms(frame.Position, frame.QuaternionRotation, frame.Scale);
                    }
                }

                index++;
            }
        }
    }
}
