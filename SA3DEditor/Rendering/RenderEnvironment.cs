using SA3D.Rendering;
using SA3D.Rendering.Input;
using SA3D.Rendering.Shaders;
using SA3D.Rendering.UI.Debugging;
using SA3D.Rendering.Structs;
using SA3D.Common.Lookup;
using SA3D.Modeling.Mesh;
using SA3D.Modeling.Mesh.Buffer;
using SA3D.Modeling.ObjectData;
using SA3D.Texturing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using SA3D.Modeling.ObjectData.Enums;

namespace SA3DEditor.Rendering
{
    public class RenderEnvironment
    {
        private readonly HashSet<BufferMesh> _toDebuffer = new();
        private readonly HashSet<BufferMesh> _toBuffer = new();

        private LandTable? _landtable;
        private TextureSet? _landTableTextures;
        private readonly Dictionary<string, RenderTask> _tasks;

        public RenderContext Context { get; }
        public DebugController DebugController { get; }
        public DebugOverlay Overlay => DebugController.Overlay;
        public CameraController CameraController { get; }

        public LandTable? LandTable
        {
            get => _landtable;
            set
            {
                if(_landtable == value)
                {
                    return;
                }

                if(_landtable != null)
                {
                    _toDebuffer.UnionWith(_landtable.Geometry
                        .SelectMany(x => x.Model.GetTreeAttachEnumerable())
                        .SelectMany(x => x.MeshData));
                }

                _landtable = value;

                if(_landtable != null)
                {
                    _landtable.BufferMeshData(true);

                    _toBuffer.UnionWith(_landtable.Geometry
                        .SelectMany(x => x.Model.GetTreeAttachEnumerable())
                        .SelectMany(x => x.MeshData));
                    _toDebuffer.ExceptWith(_toBuffer);
                }
                else
                {
                    ActiveLandEntry = null;
                }

                Context.DebufferMeshes(_toDebuffer.ToArray());
                Context.BufferMeshes(_toBuffer.ToArray());

                _toDebuffer.Clear();
                _toBuffer.Clear();
            }
        }

        public ILabeledArray<LandEntry> Geometry => LandTable?.Geometry ?? new LabeledArray<LandEntry>(0);

        public LandEntry[] VisualGeometry
            => Geometry.Where(x => x.SurfaceAttributes.HasFlag(SurfaceAttributes.Visible)).ToArray();

        public LandEntry[] CollisionGeometry
            => Geometry.Where(x => x.SurfaceAttributes.CheckIsCollision()).ToArray();

        public bool RenderCollisions { get; set; }

        public LandEntry? ActiveLandEntry { get; set; }

        public TextureSet? LandTableTextures
        {
            get => _landTableTextures;
            set
            {
                if(_landTableTextures == value)
                {
                    return;
                }

                if(_landTableTextures != null)
                {
                    Context.UnloadTextureSet(_landTableTextures);
                }

                _landTableTextures = value;

                if(_landTableTextures != null)
                {
                    Context.LoadTextureSet(_landTableTextures);
                }
            }
        }

        public ReadOnlyDictionary<string, RenderTask> Tasks { get; }

        public Node? ActiveNode { get; set; }


        public RenderEnvironment(Size viewport)
        {
            Context = new(viewport);
            DebugController = new(Context);
            CameraController = new(Context.Input, Context.Camera);
            Context.BackgroundColor = new(0x7F, 0x7F, 0x7F);

            _tasks = new();
            Tasks = new(_tasks);

            Context.OnInitialize += Load;
            Context.OnUpdate += (c, d) => DebugController.Run(d);
            Context.OnUpdate += Update;
            Context.OnRender += Render;

            Context.AddCanvas(DebugController.Overlay);
        }


        private void Load(RenderContext context)
        {

        }

        private void Update(RenderContext context, double delta)
        {
            CameraController.Run(delta);

            foreach(RenderTask task in _tasks.Values)
            {
                task.Update((float)delta);
            }
        }

        private void Render(RenderContext context)
        {
            context.SetMeshShader(Shaders.SurfaceDebug);
            context.ActiveTextures = LandTableTextures;
            RenderLandEntries(RenderCollisions ? CollisionGeometry : VisualGeometry, context);

            foreach(RenderTask task in Tasks.Values)
            {
                context.ActiveTextures = task.Textures;
                context.RenderModel(task.Model, ActiveNode);
            }
        }

        private void RenderLandEntries(LandEntry[] entries, RenderContext context)
        {
            Dictionary<Attach, List<LandEntry>> renderGroups = new();

            foreach(LandEntry entry in entries)
            {
                if(entry.Model.Attach == null)
                {
                    continue;
                }

                if(!renderGroups.TryGetValue(entry.Model.Attach, out List<LandEntry>? group))
                {
                    group = new();
                    renderGroups.Add(entry.Model.Attach, group);
                }

                group.Add(entry);
            }

            foreach(KeyValuePair<Attach, List<LandEntry>> item in renderGroups)
            {
                RenderMatrices[] matrices = item.Value
                    .Select(x => new RenderMatrices(
                        x.Model.LocalMatrix,
                        context.Camera.GetMVPMatrix(x.Model.LocalMatrix)))
                    .ToArray();

                context.RenderMeshes(item.Key.MeshData, matrices);
            }
        }


        public RenderTask CreateTask(Node model, string name)
        {
            RenderTask task = new(this, model, name);
            return _tasks.TryAdd(name, task) ? task : throw new InvalidOperationException("That task name is already taken!");
        }

        public void ClearTasks()
        {
            string[] taskNames = _tasks.Keys.ToArray();
            foreach(string name in taskNames)
            {
                RemoveTask(name);
            }
        }

        public void RemoveTask(string name)
        {
            if(!_tasks.TryGetValue(name, out RenderTask? task))
            {
                return;
            }

            if(task.Textures != null)
            {
                Context.UnloadTextureSet(task.Textures);
            }

            Node[] nodes = task.Model.GetTreeNodes();
            if(nodes.Contains(ActiveNode))
            {
                ActiveNode = null;
            }

            _tasks.Remove(name);
        }

        public void RemoveTask(RenderTask task)
        {
            RemoveTask(task.Name);
        }

        public void Reset()
        {
            LandTable = null;
            LandTableTextures = null;
            ClearTasks();
        }
    }
}
