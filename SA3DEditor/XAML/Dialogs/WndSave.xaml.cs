using Microsoft.Win32;
using SA3D.Modeling.Mesh;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.ObjectData.Enums;
using SA3DEditor.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SA3DEditor.XAML.Dialogs
{
    /// <summary>
    /// Interaction logic for WndSave.xaml
    /// </summary>
    public partial class WndSave : Window
    {
        /// <summary>
        /// Dialog passed to the filepath button
        /// </summary>
        private readonly SaveFileDialog _sfd;

        /// <summary>
        /// The attach format to save in
        /// </summary>
        private ModelFormat _format;

        /// <summary>
        /// The current appmode, which determines which file formats are available
        /// </summary>
        private readonly Mode _appMode;

        /// <summary>
        /// The current file extension
        /// </summary>
        private string? _currentFileExtension;

        /// <summary>
        /// Path of the selected file
        /// </summary>
        public string? Filepath { get; set; }

        /// <summary>
        /// Format to save the data in
        /// </summary>
        public ModelFormat Format { get; private set; }

        /// <summary>
        /// Whether the file is an NJ file
        /// </summary>
        public bool NJ { get; private set; }

        /// <summary>
        /// Whether the output should be optimized
        /// </summary>
        public bool Optimize { get; private set; }



        public WndSave(Mode appMode, string? lastFilePath, ModelFormat lastFormat, bool lastNJ, bool lastOptimized)
        {
            if (appMode is not Mode.Level and not Mode.Model)
            {
                throw new NotImplementedException("Only model and level files can be saved atm");
            }

            InitializeComponent();

            _sfd = new()
            {
                OverwritePrompt = false
            };

            FilepathControl.Dialog = _sfd;

            if (!string.IsNullOrWhiteSpace(lastFilePath))
            {
                _sfd.InitialDirectory = Path.GetDirectoryName(lastFilePath);
                Filepath = lastFilePath;
                _format = lastFormat;
                FormatControl.SelectedIndex = (int)lastFormat;
                NJFormatControl.IsChecked = lastNJ;
                OptimizeControl.IsChecked = lastOptimized;
            }
            else
            {
                _format = ModelFormat.Buffer;
            }

            _appMode = appMode;

            switch (appMode)
            {
                case Mode.Model:
                    Title = "Save Model File";
                    break;
                case Mode.Level:
                    Title = "Save Level File";
                    break;
                case Mode.ProjectSA1:
                case Mode.ProjectSA2:
                case Mode.None:
                default:
                    break;
            }

            RefreshFileExtension();
        }

        private void RefreshFileExtension()
        {
            NJFormatControl.IsEnabled = _format is not ModelFormat.Buffer and not ModelFormat.SA2B && _appMode == Mode.Model;
            switch (_format)
            {
                case ModelFormat.Buffer:
                    NJFormatControl.IsChecked = false;
                    if (_appMode == Mode.Model)
                    {
                        _currentFileExtension = "bfmdl";
                        _sfd.Filter = "Buffer Model (*.bfmdl)|*.bfmdl";
                    }
                    else if (_appMode == Mode.Level)
                    {
                        _currentFileExtension = "bflvl";
                        _sfd.Filter = "Buffer Level (*.bflvl)|*.bflvl";
                    }

                    break;
                case ModelFormat.SA1:
                case ModelFormat.SADX:
                    if (_appMode == Mode.Model)
                    {
                        _sfd.Filter = NJFormatControl.IsChecked == true ?
                            "BASIC Ninja file (*.nj)|*.nj" :
                            "Sonic Adventure 1 Model (*.sa1mdl)|*.sa1mdl";

                        _currentFileExtension = NJFormatControl.IsChecked == true ? "nj" : "sa1mdl";
                    }
                    else if (_appMode == Mode.Level)
                    {
                        _sfd.Filter = "Sonic Adventure 1 Level (*.sa1lvl)|*.sa1lvl";
                        _currentFileExtension = "sa1lvl";
                    }

                    break;
                case ModelFormat.SA2:
                    if (_appMode == Mode.Model)
                    {
                        _sfd.Filter = NJFormatControl.IsChecked == true ?
                            "BASIC Ninja file (*.nj)|*.nj" :
                            "Sonic Adventure 2 Model (*.sa2mdl)|*.sa2mdl";

                        _currentFileExtension = NJFormatControl.IsChecked == true ? "nj" : "sa2mdl";
                    }
                    else if (_appMode == Mode.Level)
                    {
                        _currentFileExtension = "sa2lvl";
                        _sfd.Filter = "Sonic Adventure 2 Level (*.sa2lvl)|*.sa2lvl";
                    }

                    break;
                case ModelFormat.SA2B:
                    NJFormatControl.IsChecked = false;
                    if (_appMode == Mode.Model)
                    {
                        _sfd.Filter = "Sonic Adventure 2 Battle Model (*.sa2bmdl)|*.sa2bmdl";
                        _currentFileExtension = "sa2bmdl";
                    }
                    else if (_appMode == Mode.Level)
                    {
                        _sfd.Filter = "Sonic Adventure 2 Battle Level (*.sa2blvl)|*.sa2blvl";
                        _currentFileExtension = "sa2blvl";
                    }

                    break;
                default:
                    throw new ArgumentException("No valid Attach format was passed");
            }

            Filepath = Path.ChangeExtension(Filepath, _currentFileExtension) ?? throw new NullReferenceException("No Filepath!");
            FilepathControl.FilePath = Filepath;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Filepath))
            {
                _ = MessageBox.Show("Please select a path to write to.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (File.Exists(Filepath))
            {
                MessageBoxResult r = MessageBox.Show($"\"{Filepath}\"already exists.\n Do you want to overwrite it?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r is MessageBoxResult.No or MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            Format = _format;
            NJ = NJFormatControl.IsChecked == true;
            Optimize = OptimizeControl.IsChecked == true;
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Filepath = null;
            DialogResult = null;
            Close();
        }

        private void NJFormat_Click(object sender, RoutedEventArgs e)
        {
            RefreshFileExtension();
        }

        private void Format_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox s = (ComboBox)sender;

            ModelFormat newFormat = (ModelFormat)s.SelectedIndex;
            if (_format == newFormat)
            {
                return;
            }

            _format = newFormat;
            RefreshFileExtension();
        }

        private void FilepathControl_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox fp = (TextBox)sender;

            string newPath = fp.Text;
            if (newPath == "")
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(newPath))
            {
                fp.Text = "";
                return;
            }

            string? extension = Path.GetExtension(fp.Text);
            if (extension != _currentFileExtension)
            {
                fp.Text = Path.ChangeExtension(fp.Text, _currentFileExtension);
            }
        }
    }
}
