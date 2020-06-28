namespace Winecrash.Launcher
{
    partial class frmLauncher_NoBrowser
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLauncher_NoBrowser));
            this.label1 = new System.Windows.Forms.Label();
            this.cbVersions = new System.Windows.Forms.ComboBox();
            this.btDownload = new System.Windows.Forms.Button();
            this.progBarDownload = new System.Windows.Forms.ProgressBar();
            this.lbDownloadSpeed = new System.Windows.Forms.Label();
            this.lbDownloadQty = new System.Windows.Forms.Label();
            this.btPlay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version";
            // 
            // cbVersions
            // 
            this.cbVersions.FormattingEnabled = true;
            this.cbVersions.Items.AddRange(new object[] {
            "Last"});
            this.cbVersions.Location = new System.Drawing.Point(15, 25);
            this.cbVersions.Name = "cbVersions";
            this.cbVersions.Size = new System.Drawing.Size(121, 21);
            this.cbVersions.TabIndex = 1;
            this.cbVersions.SelectedIndexChanged += new System.EventHandler(this.cbVersions_SelectedIndexChanged);
            // 
            // btDownload
            // 
            this.btDownload.Enabled = false;
            this.btDownload.Location = new System.Drawing.Point(142, 23);
            this.btDownload.Name = "btDownload";
            this.btDownload.Size = new System.Drawing.Size(75, 23);
            this.btDownload.TabIndex = 2;
            this.btDownload.Text = "Download";
            this.btDownload.UseVisualStyleBackColor = true;
            this.btDownload.Click += new System.EventHandler(this.btDownload_Click);
            // 
            // progBarDownload
            // 
            this.progBarDownload.Enabled = false;
            this.progBarDownload.Location = new System.Drawing.Point(15, 52);
            this.progBarDownload.Name = "progBarDownload";
            this.progBarDownload.Size = new System.Drawing.Size(383, 23);
            this.progBarDownload.TabIndex = 3;
            // 
            // lbDownloadSpeed
            // 
            this.lbDownloadSpeed.AutoSize = true;
            this.lbDownloadSpeed.Location = new System.Drawing.Point(223, 28);
            this.lbDownloadSpeed.Name = "lbDownloadSpeed";
            this.lbDownloadSpeed.Size = new System.Drawing.Size(54, 13);
            this.lbDownloadSpeed.TabIndex = 4;
            this.lbDownloadSpeed.Text = "125 MB/s";
            // 
            // lbDownloadQty
            // 
            this.lbDownloadQty.AutoSize = true;
            this.lbDownloadQty.Location = new System.Drawing.Point(302, 28);
            this.lbDownloadQty.Name = "lbDownloadQty";
            this.lbDownloadQty.Size = new System.Drawing.Size(88, 13);
            this.lbDownloadQty.TabIndex = 5;
            this.lbDownloadQty.Text = "500 KB / 800 KB";
            // 
            // btPlay
            // 
            this.btPlay.Enabled = false;
            this.btPlay.Location = new System.Drawing.Point(15, 495);
            this.btPlay.Name = "btPlay";
            this.btPlay.Size = new System.Drawing.Size(383, 65);
            this.btPlay.TabIndex = 7;
            this.btPlay.Text = "Play";
            this.btPlay.UseVisualStyleBackColor = true;
            this.btPlay.Click += new System.EventHandler(this.btPlay_Click);
            // 
            // frmLauncher_NoBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 572);
            this.Controls.Add(this.btPlay);
            this.Controls.Add(this.lbDownloadQty);
            this.Controls.Add(this.lbDownloadSpeed);
            this.Controls.Add(this.progBarDownload);
            this.Controls.Add(this.btDownload);
            this.Controls.Add(this.cbVersions);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmLauncher_NoBrowser";
            this.Text = "Winecrash Launcher";
            this.Load += new System.EventHandler(this.frmLauncher_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbVersions;
        private System.Windows.Forms.Button btDownload;
        private System.Windows.Forms.ProgressBar progBarDownload;
        private System.Windows.Forms.Label lbDownloadSpeed;
        private System.Windows.Forms.Label lbDownloadQty;
        private System.Windows.Forms.Button btPlay;
    }
}

