using SA3DEditor.Rendering;
using System.Windows;

namespace SA3DEditor.XAML
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App(RenderEnvironment environment)
        {
            InitializeComponent();
            MainWindow = new WndMain(environment);
            MainWindow.Show();
        }
    }
}
