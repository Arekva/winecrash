using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;
using System.Reflection;
using System.Threading.Tasks;

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
            _status.Text = "";
            _pathInput.Select(_pathInput.Text.Length,0);

            // no shortcut on linux/osx yet.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                _createDesktopShortcut.Checked = _createStartShortcut.Checked =
                _createDesktopShortcut.Enabled = _createStartShortcut.Enabled = false;
            }
            
            _client.DownloadFileCompleted += _client_DownloadFileCompleted;
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
            _status.Text = $"Downloading {e.ProgressPercentage}% ({Utilities.FormatSize(e.BytesReceived)}/{Utilities.FormatSize(e.TotalBytesToReceive)})";
        }

        private void SafeInvoke(Action a) => Invoke(a);

        private void _client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _cancelButton.Enabled = false;
            string zipPath = Utilities.ZipPath;
            
            if (e.Cancelled)
            {
                _status.Text = "Deleting temporary file";
                File.Delete(zipPath);
                MessageBox.Show("Install of the launcher has been canceled.", "Install canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _status.Text = "Install canceled";
                _installProgress.Value = 0;
            }
            else
            {
                _cancelButton.Enabled = false;
                _status.Text = "Extracting";
                Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(zipPath, Utilities.LauncherPath);

                    SafeInvoke(() =>
                    {
                        _status.Text = "Deleting temporary file";
                        File.Delete(zipPath);
                        
                        _status.Text = "Creating shortcuts";
                        if (_createDesktopShortcut.Checked)
                            Utilities.CreateShortcutDesktop(Utilities
                                .LauncherExecutablePath);
                        if (_createStartShortcut.Checked)
                            Utilities.CreateShortcutStart(Utilities
                                .LauncherExecutablePath);

                        _status.Text = "Install complete";
                        
                        _installProgress.Value = 0;
                                                
                        MessageBox.Show("Install of the launcher successfully completed.", "Install completed",
                                                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        if (_openInstall.Checked)
                        {
                            ProcessStartInfo infos = new ProcessStartInfo(Utilities.LauncherExecutablePath)
                            {
                                WorkingDirectory = Utilities.LauncherPath
                            };

                            Process.Start(infos);
                        }
                    });
                });
            }
        }

        private void _buttonInstall_Click(object sender, EventArgs e)
        {
            string path = _pathInput.Text;
            if (Utilities.InstalledAtPath(Path.Combine(path, Utilities.ApplicationName)))
            {
                DialogResult result =
                    MessageBox.Show(
                        $"{Utilities.ApplicationName} is already installed at this directory. Do you want to replace it?", "Directory conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

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