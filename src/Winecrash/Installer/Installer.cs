using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;
using System.Reflection;

namespace Winecrash.Installer
{
    public partial class Installer : Form
    {
        private WebClient _client = new WebClient();
        
        public Installer()
        {
            InitializeComponent();
        }

        private void Installer_Load(object sender, EventArgs e)
        {
            _pathInput.Text = Utilities.DefaultInstallDirectory;
            _client.DownloadFileCompleted += _client_InstallFileCompleted;
            _client.DownloadProgressChanged += _client_DownloadProgress;
        }

        public void InstallLauncher(string path)
        {
            Utilities.SetupForInstallation(path);
            
            _cancelButton.Enabled = true;
            
            _client.DownloadFileAsync(new Uri(Utilities.DownloadLink), Utilities.ZipPath);
        }

        private void _pathInput_TextChanged(object sender, EventArgs e)
        {
            _actualInstallationLabel.Text = Path.Combine(_pathInput.Text, Utilities.ApplicationName);
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _client.CancelAsync();
        }

        private void _client_DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            _installProgress.Value = e.ProgressPercentage;
        }

        private void _client_InstallFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _cancelButton.Enabled = false;
            
            if (e.Cancelled)
            {
                MessageBox.Show("Installation Cancelled");
            }
            else
            {
                string zipPath = Utilities.ZipPath;

                ZipFile.ExtractToDirectory(zipPath, Utilities.LauncherPath);

                File.Delete(zipPath);
                if (_createDesktopShortcut.Checked)
                    Utilities.CreateShortcutDesktop(Utilities.LauncherExecutablePath);//Path.Combine(Utilities.LauncherPath, "Winecrash.exe"));
                if (_createStartShortcut.Checked)
                    Utilities.CreateShortcutStart(Utilities.LauncherExecutablePath);//Path.Combine(Utilities.LauncherPath, "Winecrash.exe"));
            
                
            }
        }

        private void _buttonInstall_Click(object sender, EventArgs e)
        {
            string path = _pathInput.Text;
            if (Utilities.InstalledAtPath(Path.Combine(path, Utilities.ApplicationName)))
            {
                DialogResult result =
                    MessageBox.Show(
                        $"{Utilities.ApplicationName} is already installed at this directory. Do you want to replace it?", "Directory Conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (result == DialogResult.Yes)
                {
                    Directory.Delete(Path.Combine(path, Utilities.ApplicationName), true);
                    InstallLauncher(path);
                }
            }
            else
                InstallLauncher(path);
        }
    }
}