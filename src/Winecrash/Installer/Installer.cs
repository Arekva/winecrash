using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Reflection;

namespace Winecrash.Installer
{
    public partial class Installer : Form
    {
        public Installer()
        {
            InitializeComponent();
        }

        private void Installer_Load(object sender, EventArgs e)
        {
            InstallLauncher(Utilities.DefaultInstallDirectory);
        }

        public void InstallLauncher(string path)
        {
            /*string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            foreach (string name in names)
            {
                MessageBox.Show(name);
            }*/
            
            Utilities.SetupForInstallation(path);
            string zipPath = Path.Combine(Utilities.ApplicationPath, "temp.zip");

            using (Stream cs = Assembly.GetExecutingAssembly().GetManifestResourceStream(Utilities.EmbeddedFileName))
            using (FileStream fs = new FileStream(zipPath, FileMode.Create)) cs.CopyTo(fs);
            
            ZipFile.ExtractToDirectory(zipPath, Utilities.LauncherPath);
            
            File.Delete(zipPath);
            /*using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                zipStream.
            }*/
        }
    }
}