using SA3D.Archival;
using SA3D.Rendering;
using SA3D.Modeling.Animation;
using SA3D.Texturing;
using SA3DEditor.Rendering;
using System;
using System.IO;
using System.Runtime.InteropServices;
using SA3D.Modeling.File;
using SA3D.Common.WPF.ErrorHandling;

namespace SA3DEditor
{
    public class Program
    {
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        public static ErrorHandler ErrorHandler { get; }

        static Program()
        {
            ErrorHandler = new("https://github.com/X-Hax/SA3DEditor/issues");
        }

        [STAThread]
        public static void Main(string[] args)
        {
            AttachConsole(-1);
            Run(args);
            FreeConsole();
        }

        private static void Run(string[] args)
        {
            // when running from cmd, attach to the cmd console

            string path = "";

            if(args.Length > 0)
            {
                if(args[0].StartsWith("-"))
                {
                    args[0] = "?";
                }

                switch(args[0])
                {
                    case "?":
                        string output = "";

                        output += "\nSA3D Standalone @X-Hax\n";
                        output += "  Usage: [filepath] [options]\n\n";

                        output += "   filepath\n";
                        output += "       Path to a sonic adventure level or model file that should be opened\n\n";

                        output += "  Options:\n";
                        output += "   -h --help           Help \n\n";

                        output += "   -tex --textures\n";
                        output += "       Loads a texture archive.\n\n";

                        output += "   -mtn --motion\n";
                        output += "       Loads a motion file and attaches it to the loaded model.\n\n\n";

                        output += "   -st  --standlone\n";
                        output += "       Starts SA3D as a standalone window (only used for model inspection).\n\n";

                        output += "   -res --resolution   [Width]x[Height]\n";
                        output += "       Used to start the standalone with specific dimensions.\n\n";

                        Console.WriteLine(output);
                        return;
                    default:
                        path = Path.Combine(Environment.CurrentDirectory, args[0]);
                        if(!File.Exists(path))
                        {
                            Console.WriteLine("Path does not lead to a file! enter --help for more info");
                            return;
                        }

                        break;
                }
            }

            RenderEnvironment env = new(new(1280, 720));

            string? motionPath = null;
            string? texturePath = null;
            bool standalone = false;

            for(int i = 1; i < args.Length; i++)
            {
                switch(args[i].ToLower())
                {
                    case "-res":
                    case "--resolution":
                        i++;
                        string[] res = args[i].Split('x');
                        if(!int.TryParse(res[0], out int width) || !int.TryParse(res[1], out int height))
                        {
                            Console.WriteLine("Resolution not valid:\n -res [WIDTH]x[HEIGHT]\n  example: 1280x720");
                            return;
                        }

                        env.Context.Viewport = new(width, height);
                        break;
                    case "-st":
                    case "--standalone":
                        standalone = true;
                        break;
                    case "-tex":
                    case "--textures":
                        i++;
                        texturePath = args[i];

                        texturePath = Path.Combine(Environment.CurrentDirectory, texturePath);
                        if(!File.Exists(path))
                        {
                            Console.WriteLine("Texture filepath does not lead to a file!");
                            return;
                        }

                        break;
                    case "-mtn":
                    case "--motion":
                        i++;
                        motionPath = args[i];

                        motionPath = Path.Combine(Environment.CurrentDirectory, motionPath);
                        if(!File.Exists(path))
                        {
                            Console.WriteLine("Motion filepath does not lead to a file!");
                            return;
                        }

                        break;
                }
            }

            // loading the model file
            if(path != null)
            {
                TextureSet? textures = null;

                if(texturePath != null)
                {
                    textures = Archive.ReadArchiveFromFile(texturePath).ToTextureSet();
                }

                string ext = Path.GetExtension(path);
                if(ext.EndsWith("lvl"))
                {
                    env.LandTable = LevelFile.ReadFromFile(path).Level;
                    env.LandTableTextures = textures;
                }
                else if(ext.EndsWith("mdl") || ext.EndsWith("nj"))
                {
                    ModelFile file = ModelFile.ReadFromFile(path);

                    RenderTask task = env.CreateTask(file.Model, file.Model.Label);
                    task.Textures = textures;

                    if(motionPath != null)
                    {
                        int animNodeCount = file.Model.GetAnimTreeNodeCount();
                        Motion motion = AnimationFile.ReadFromFile(motionPath, (uint)animNodeCount, false).Animation;
                        task.Animations.Add(new(motion, 30));
                    }
                }
                else
                {
                    Console.WriteLine($"Not a valid file format: {ext}");
                }
            }

            if(standalone)
            {
                new RenderWindow(env.Context).Run();
            }
            else
            {
                AppDomain.CurrentDomain.UnhandledException += ErrorHandler.OnUnhandledException;

                XAML.App app = new(env);
                app.DispatcherUnhandledException += (o, e) =>
                {
                    ErrorHandler.HandleException(e.Exception);
                    e.Handled = true;
                };
                app.InitializeComponent();
                app.Run();

            }

        }
    }
}
