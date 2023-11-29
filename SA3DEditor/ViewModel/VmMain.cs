using SA3D.Modeling.Mesh;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.Animation;
using SA3DEditor.ViewModel.TreeItems;
using SA3D.Texturing;
using SA3D.Common.WPF.MVVM;
using SA3DEditor.Rendering;
using System;
using System.IO;
using System.Linq;
using SA3D.Modeling.ObjectData.Enums;
using SA3D.Modeling.File;
using SA3D.Common.Lookup;
using SA3D.Modeling.Mesh.Weighted;

namespace SA3DEditor.ViewModel
{
    public enum Mode
    {
        None,
        Model,
        Level,
        ProjectSA1,
        ProjectSA2
    }

    /// <summary>
    /// Main view model used to control the entire application
    /// </summary>
    public class VmMain : BaseViewModel
    {
        public RenderEnvironment Environment { get; }

        #region Application mode Stuff

        private Mode _appMode;

        /// <summary>
        /// Application mode
        /// </summary>
        public Mode ApplicationMode
        {
            get => _appMode;
            private set
            {
                _appMode = value;
                OnPropertyChanged(nameof(ApplicationModeNotNone));
                OnPropertyChanged(nameof(EnableObjectTab));
                OnPropertyChanged(nameof(EnableGeometryTab));
            }
        }

        /// <summary>
        /// Whether the application mode hasnt been set yet
        /// </summary>
        public bool ApplicationModeNotNone
            => ApplicationMode != Mode.None;

        /// <summary>
        /// Whether to get Access to the model tab
        /// </summary>
        public bool EnableObjectTab
            => ApplicationMode is not Mode.Level and not Mode.None;

        /// <summary>
        /// Whether to get Access to the Geometry Tab
        /// </summary>
        public bool EnableGeometryTab
            => ApplicationMode is not Mode.Model and not Mode.None;

        #endregion

        #region File information

        /// <summary>
        /// The path to the currently opened file
        /// </summary>
        public string? FilePath { get; private set; }

        /// <summary>
        /// The attach format of the currently opened file
        /// </summary>
        public ModelFormat FileFormat { get; private set; }

        /// <summary>
        /// Whether the currently opened file is a ninja file
        /// </summary>
        public bool FileIsNJ { get; private set; }

        /// <summary>
        /// Whether the output file should be optimized
        /// </summary>
        public bool FileOptimize { get; private set; }

        /// <summary>
        /// File information for the window border
        /// </summary>
        public string WindowTitle =>
            string.IsNullOrWhiteSpace(FilePath) ? "SA3D" : $"SA3D [{FilePath}] | {FileFormat}"
                + (FileIsNJ ? " | NJ" : "") + (FileOptimize ? " | Optimized" : "");

        #endregion

        /// <summary>
        /// Object treeview data
        /// </summary>
        public VmDataTree ObjectTree { get; }

        /// <summary>
        /// Geometry treeview data
        /// </summary>
        public VmDataTree GeometryTree { get; }

        public VmMain(RenderEnvironment environment)
        {
            Environment = environment;
            ObjectTree = new VmDataTree(this);
            GeometryTree = new VmDataTree(this);
        }

        public void New3DFile(Mode mode)
        {
            ApplicationMode = mode;
            ObjectTree.Reset();
            GeometryTree.Reset();
            Environment.Reset();

            FilePath = null;
            FileOptimize = true;
            FileFormat = ModelFormat.Buffer;
            FileIsNJ = false;

            switch(mode)
            {
                case Mode.Model:
                    Node obj = new()
                    {
                        Label = "Root"
                    };

                    RenderTask task = Environment.CreateTask(obj, obj.Label);
                    ObjectTree.Items.Add(new VmTask(this, null, task, null));
                    break;
                case Mode.Level:
                    LoadLandtable(new LandTable(new LabeledArray<LandEntry>(0), ModelFormat.Buffer));
                    break;
                case Mode.None:
                case Mode.ProjectSA1:
                case Mode.ProjectSA2:
                default:
                    throw new NotImplementedException("Project modes not yet implemented");
            }
        }

        public bool OpenFile(string filepath)
        {
            byte[] file = File.ReadAllBytes(filepath);

            if(ModelFile.CheckIsModelFile(file))
            {
                ModelFile mdlFile = ModelFile.ReadFromBytes(file);

                LoadModel(mdlFile.Model, Path.GetFileNameWithoutExtension(filepath), mdlFile);

                FilePath = filepath;
                FileOptimize = false;
                FileFormat = mdlFile.Format;
                FileIsNJ = mdlFile.NJFile;
                return true;
            }
            else if(LevelFile.CheckIsLevelFile(file))
            {
                LevelFile level = LevelFile.ReadFromBytes(file);

                LoadLandtable(level.Level);

                FilePath = filepath;
                FileOptimize = false;
                FileIsNJ = false;
                FileFormat = level.Level.Format;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears the current scene and loads the model
        /// </summary>
        /// <param name="task"></param>
        private void LoadModel(Node model, string name, ModelFile? modelinfo)
        {
            ApplicationMode = Mode.Model;

            Environment.Reset();
            RenderTask task = Environment.CreateTask(model, name);

            ObjectTree.Reset();
            GeometryTree.Reset();
            ObjectTree.Items.Add(new VmTask(this, null, task, modelinfo));
        }

        public void LoadAnimation(RenderMotion motion)
        {
            VmTask task = GetTargetTask();
            task.Task.Animations.Add(motion);
            task.Refresh();
        }

        public void LoadTextures(TextureSet textures)
        {
            if(ApplicationMode == Mode.Level)
            {
                Environment.LandTableTextures = textures;
            }
            else if(ApplicationMode == Mode.Model)
            {
                VmTask task = GetTargetTask();
                task.Task.Textures = textures;
            }
            else
            {

            }
        }

        /// <summary>
        /// Clears the current scene and loads a landtable
        /// </summary>
        /// <param name="ltbl">The landtable to load</param>
        private void LoadLandtable(LandTable ltbl)
        {
            ApplicationMode = Mode.Level;
            Environment.Reset();
            Environment.LandTable = ltbl;

            ObjectTree.Reset();
            GeometryTree.Reset();

            GeometryTree.Items.Add(new VmLandEntryHead(this, null, Environment.VisualGeometry, false));
            GeometryTree.Items.Add(new VmLandEntryHead(this, null, Environment.CollisionGeometry, true));
            GeometryTree.Items.Add(new VmTextureHead(this, null, Environment.LandTableTextures));
        }

        private VmTask GetTargetTask()
        {
            if(ObjectTree.Selected != null)
            {
                return ObjectTree.Selected.TryGetParent<VmTask>()
                    ?? throw new InvalidOperationException("Selected tree item is not part of a task!");
            }
            else if(Environment.Tasks.Count == 1)
            {
                return (VmTask)(ObjectTree.Items.First(x => x is VmTask)
                    ?? throw new InvalidOperationException("Object tree has no task item"));
            }

            throw new InvalidOperationException("Multiple tasks found! Please select the target node tree");
        }

        public void InsertModel(Node insertRoot, bool insertAtRoot, TextureSet? textures, RenderMotion[] animations)
        {
            if(ApplicationMode != Mode.Model)
            {
                return;
            }

            VmTask vmTask = GetTargetTask();
            VmNode? vmNode = null;
            if(ObjectTree.Selected is VmNode selectedNode)
            {
                vmNode = selectedNode;
            }

            if(insertAtRoot && vmNode != null && vmNode.Parent?.ItemType != TreeItemType.NodeHead)
            {
                vmNode.NodeData.AppendChild(insertRoot);
            }
            else if(vmTask.Task.Model.ChildCount == 0)
            {
                vmTask.Task.ReplaceModel(insertRoot);
                vmTask.Task.Textures = textures;
                vmTask.Task.Animations.AddRange(animations);
            }
            else
            {
                vmTask.Task.Model.AppendChild(insertRoot);
                vmTask.Task.Model.BufferMeshData(false);
            }

        }

        public void SaveToFile(string filepath, ModelFormat format, bool nj, bool optimize)
        {
            FilePath = filepath;
            FileFormat = format;
            bool forceUpdate = optimize != FileOptimize;
            FileIsNJ = nj;
            FileOptimize = optimize;
            SaveToFile(forceUpdate);
        }

        public void SaveToFile(bool forceUpdate = false)
        {
            if(FilePath == null)
            {
                throw new InvalidOperationException("No filepath set!");
            }

            if(ApplicationMode == Mode.Model)
            {
                VmTask vmTask = GetTargetTask();

                AttachFormat format = FileFormat switch
                {
                    ModelFormat.SA1 or ModelFormat.SADX => AttachFormat.BASIC,
                    ModelFormat.SA2 => AttachFormat.CHUNK,
                    ModelFormat.SA2B => AttachFormat.GC,
                    _ => AttachFormat.Buffer,
                };

                vmTask.Task.Model.ConvertAttachFormat(format, BufferMode.None, FileOptimize, false, forceUpdate);
                ModelFile.WriteToFile(FilePath, vmTask.Task.Model, FileIsNJ, format: FileFormat);
            }
            else if(ApplicationMode == Mode.Level)
            {
                LandTable ltbl = Environment.LandTable ?? throw new NullReferenceException("No landtable loaded");
                ltbl.ConvertToFormat(FileFormat, BufferMode.None, FileOptimize, forceUpdate, true);

                // conversion to sa2/b may have added geometry right at the end
                for(int i = Environment.Geometry.Length; i < ltbl.Geometry.Length; i++)
                {
                    Environment.Geometry.Add(ltbl.Geometry[i]);
                }

                LevelFile.WriteToFile(FilePath, ltbl);
            }
            else
            {
                throw new NotImplementedException("Project saving not yet supported");
            }
        }
    }
}
