using Microsoft.Win32;
using SA3D.Archival;
using SA3D.Rendering.WPF;
using SA3D.Rendering.WPF.XAML;
using SA3D.Modeling.Animation;
using SA3DEditor.XAML.Dialogs;
using SA3D.Texturing;
using SA3DEditor.Rendering;
using SA3DEditor.Properties;
using SA3DEditor.ViewModel;
using System;
using System.Windows;
using SA3D.Modeling.File;
using System.Windows.Media;
using SA3D.Common.WPF.Utilities;

namespace SA3DEditor.XAML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WndMain : Window
    {
        private VmMain Main { get; }

        public WndMain(RenderEnvironment environment)
        {
            Main = new VmMain(environment);
            DataContext = Main;
            InitializeComponent();

            maingrid.Children.Add(new RenderControl(environment.Context));
        }

        private void ControlSettings_Click(object sender, RoutedEventArgs e)
        {
            new WndInputSettings(Settings.Default).ShowDialog();
            Settings.Default.ApplyToController(Main.Environment.DebugController);
            Settings.Default.ApplyToController(Main.Environment.CameraController);
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Filter = "All (*.*mdl, *.nj, *.gj, *.*lvl, *.prs)|*.BFMDL;*.SA1MDL;*.SA2MDL;*.SA2BMDL;*.NJ;*.GJ;*.BFLVL;*.SA1LVL;*.SA2LVL;*.SA2BLVL;*.PRS|Model File (*.*mdl, *.nj, *.gj)|*.BFMDL;*.SA1MDL;*.SA2MDL;*.SA2BMDL;*.NJ;*.GJ|Level File (*.*lvl)|*.BFLVL;*.SA1LVL;*.SA2LVL;*.SA2BLVL|SA2 Event (*.prs)|*.PRS"
            };

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            if (Main.OpenFile(ofd.FileName))
            {
                return;
            }

            _ = MessageBox.Show("File not in any valid format", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void NewModel(object sender, RoutedEventArgs e)
        {
            Main.New3DFile(Mode.Model);
        }

        private void NewLevel(object sender, RoutedEventArgs e)
        {
            Main.New3DFile(Mode.Level);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Main.FilePath == null)
            {
                SaveAs(sender, e);
            }
            else
            {
                Main.SaveToFile();
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            WndSave saveDialog = new(Main.ApplicationMode, Main.FilePath, Main.FileFormat, Main.FileIsNJ, Main.FileOptimize);

            if (saveDialog.ShowDialog() != true)
            {
                return;
            }

            Main.SaveToFile(saveDialog.Filepath ?? throw new NullReferenceException("No filepath!"), saveDialog.Format, saveDialog.NJ, saveDialog.Optimize);
        }

        private void LoadAnimation(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Filter = "SA Animation (*.saanim)|*.saanim"
            };

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            Motion motion = AnimationFile.ReadFromFile(ofd.FileName).Animation;
            Main.LoadAnimation(new(motion, 30));
        }

        private void LoadTextures(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Filter = "Texture archive (*.pak;*.gvm;*.pvm;*.pvmx;*.prs;)|*.pak;*.gvm;*.pvm;*.pvmx;*.prs;"
            };

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            TextureSet set = Archive.ReadArchiveFromFile(ofd.FileName).ToTextureSet();
            Main.LoadTextures(set);
        }

        private void BackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            SA3D.Modeling.Structs.Color color = Main.Environment.Context.BackgroundColor;
            Color mediaColor = Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);

            if(ColorPickerDialog.ShowAsDialog(mediaColor, out mediaColor) == true)
            {
                Main.Environment.Context.BackgroundColor = new(mediaColor.R, mediaColor.G, mediaColor.B, mediaColor.A);
            }
        }
    }
}
