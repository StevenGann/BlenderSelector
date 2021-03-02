using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlenderSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string basePath = @"C:\Program Files\Blender Foundation\";
        public List<string> LogMessages = new List<string>();
        public List<BlenderVersion> Versions = new List<BlenderVersion>();

        public MainWindow()
        {
            InitializeComponent();
            ScanVersions();
            PopulateVersions();
        }

        private void PopulateVersions()
        {
            foreach (BlenderVersion v in Versions)
            {
                Button b = new Button
                {
                    Content = v.Name,
                    Margin = new Thickness(4, 2, 4, 2),
                    Padding = new Thickness(4),
                };
                b.Click += VersionButton_Click;
                StackPanelVersions.Children.Add(b);
            }
        }

        private void VersionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                string name = b.Content as string;
                foreach (BlenderVersion v in Versions)
                {
                    if (v.Name == name)
                    {
                        //MessageBox.Show($"{v.Name}\n{v.FullName}\n{v.Path}");
                        StartVersion(v);
                        if ((bool)CheckboxClose.IsChecked) { this.Close(); }
                    }
                }
            }
        }

        private void StartVersion(BlenderVersion Version)
        {
            string args = "";

            if ((bool)CheckboxConsole.IsChecked) { args += " -con"; }
            if ((bool)CheckboxAutoExec.IsChecked) { args += " -y"; }
            if ((bool)CheckboxBorder.IsChecked) { args += " -w"; }
            if ((bool)CheckboxFullscreen.IsChecked) { args += " -W"; }
            if ((bool)CheckboxDefault.IsChecked) { args += " --factory-startup"; }
            if ((bool)CheckboxRegister.IsChecked) { args += " -r"; }

            var p = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = $"{Version.Path}",
                    Arguments = args,
                    RedirectStandardError = false,
                    RedirectStandardOutput = false
                }
            };

            p.Start();

            if ((bool)CheckboxRegister.IsChecked)
            {
                p.WaitForExit(1000);
                CheckboxRegister.IsChecked = false;
                StartVersion(Version);
            }
        }

        private void ScanVersions()
        {
            string[] subdirectories = System.IO.Directory.GetDirectories(basePath);
            foreach (string p in subdirectories)
            {
                if (System.IO.File.Exists(p + @"\blender.exe"))
                {
                    LogMessages.Add($"Found {p.Split('\\').Last()}");
                    Versions.Add(new BlenderVersion()
                    {
                        Name = $"{p.Split('\\').Last()}",
                        Path = $"{p}\\blender.exe",
                        FullName = $"{p.Split('\\').Last()}"
                    });
                }
            }
        }

        public struct BlenderVersion
        {
            public string Name;
            public string Path;
            public string FullName;
        }
    }
}