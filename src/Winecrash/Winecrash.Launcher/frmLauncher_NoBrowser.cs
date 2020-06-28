using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Diagnostics;

using System.IO.Compression;

namespace Winecrash.Launcher
{
    public partial class frmLauncher_NoBrowser : Form
    {
        public static string WebsiteRoot = "http://docs.arthurcarre.fr/winecrash/";
        static WebClient client = new WebClient();
        static string[] builds;
        string selectedVersion;

        public frmLauncher_NoBrowser()
        {
            
            InitializeComponent();
        }

        private void frmLauncher_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory("versions");
            lbDownloadSpeed.Text = "";
            lbDownloadQty.Text = "";

            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;

            DownloadBuildJSON();

            


            for (int i = 0; i < builds.Length; i++)
            {
                cbVersions.Items.Add(builds[i]);
            }

            cbVersions.SelectedIndex = 0;
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.lbDownloadQty.Text = "";
            this.lbDownloadSpeed.Text = "";
            this.progBarDownload.Value = 0;

            ZipFile.ExtractToDirectory("versions/" + selectedVersion + ".tmp", "versions/" + selectedVersion);
            File.Delete("versions/" + selectedVersion + ".tmp");
            btPlay.Enabled = true;
            btDownload.Enabled = false;
        }

        private void cbVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cbVersions.SelectedIndex;

            selectedVersion = builds[0];

            if (idx != 0)
            {
                selectedVersion = builds[idx-1];
            }

            if (Directory.Exists("versions/" + selectedVersion))
            {
                btPlay.Enabled = true;
                btDownload.Enabled = false;
            }

            else
            {
                btPlay.Enabled = false;
                btDownload.Enabled = true;
            }
        }

        private void DownloadBuildJSON()
        {
            Uri uri = new Uri(WebsiteRoot + "builds/builds.json");
            builds = JsonConvert.DeserializeObject<string[]>(client.DownloadString(uri));
        }

        private void btPlay_Click(object sender, EventArgs e)
        {
            Process game = new Process();
            game.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "versions", selectedVersion,"Winecrash.exe");
            game.StartInfo.WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "versions", selectedVersion);
            game.Start();

            Application.Exit();
        }

        private void btDownload_Click(object sender, EventArgs e)
        {
            Uri uri = new Uri(WebsiteRoot + "builds/" + selectedVersion + ".zip");
            downloadTimeWatch.Reset();
            downloadTimeWatch.Start();
            client.DownloadFileTaskAsync(uri, "versions/" + selectedVersion + ".tmp");
        }

        Stopwatch downloadTimeWatch = new Stopwatch();
        long previoustage = 0;
        double previousTime = 0;

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            long deltaQty = e.BytesReceived - previoustage;
            double deltaTime = downloadTimeWatch.Elapsed.TotalSeconds - previousTime;
            double speed = (deltaQty / deltaTime) / 1e+3;

            this.progBarDownload.Value = e.ProgressPercentage;
            this.lbDownloadQty.Text = ((float)e.BytesReceived / 1e+6F).ToString("N2") + "/" + ((float)e.TotalBytesToReceive / 1e+6F).ToString("N2") + " MB";
            this.lbDownloadSpeed.Text = speed.ToString("N0") + " KB/s";
        }
    }
}
