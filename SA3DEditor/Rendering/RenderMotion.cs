using SA3D.Modeling.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA3DEditor.Rendering
{
    public class RenderMotion
    {
        public Motion Animation { get; }
        public uint FrameCount { get; private set; }
        public float FramesPerSecond { get; set; }

        public RenderMotion(Motion animation, float framesPerSecond)
        {
            Animation = animation;
            FrameCount = Animation.GetFrameCount();
            FramesPerSecond = framesPerSecond;
        }

        public void UpdateFrameCount()
        {
            FrameCount = Animation.GetFrameCount();
        }
    }
}
