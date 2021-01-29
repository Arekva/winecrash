using System.ComponentModel;

namespace Winecrash.Installer
{
    partial class Installer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Installer));
            this._pathInput = new System.Windows.Forms.TextBox();
            this._installLabel = new System.Windows.Forms.Label();
            this._actualInstallationLabel = new System.Windows.Forms.Label();
            this._buttonInstall = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._installProgress = new System.Windows.Forms.ProgressBar();
            this._createDesktopShortcut = new System.Windows.Forms.CheckBox();
            this._createStartShortcut = new System.Windows.Forms.CheckBox();
            this._openInstall = new System.Windows.Forms.CheckBox();
            this._status = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this._welcome = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // _pathInput
            // 
            this._pathInput.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._pathInput.Location = new System.Drawing.Point(112, 245);
            this._pathInput.Name = "_pathInput";
            this._pathInput.Size = new System.Drawing.Size(320, 20);
            this._pathInput.TabIndex = 0;
            this._pathInput.TextChanged += new System.EventHandler(this._pathInput_TextChanged);
            // 
            // _installLabel
            // 
            this._installLabel.Location = new System.Drawing.Point(12, 245);
            this._installLabel.Name = "_installLabel";
            this._installLabel.Size = new System.Drawing.Size(100, 21);
            this._installLabel.TabIndex = 1;
            this._installLabel.Text = "Install directory";
            // 
            // _actualInstallationLabel
            // 
            this._actualInstallationLabel.BackColor = System.Drawing.SystemColors.Control;
            this._actualInstallationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this._actualInstallationLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this._actualInstallationLabel.Location = new System.Drawing.Point(112, 268);
            this._actualInstallationLabel.Name = "_actualInstallationLabel";
            this._actualInstallationLabel.Size = new System.Drawing.Size(322, 36);
            this._actualInstallationLabel.TabIndex = 2;
            this._actualInstallationLabel.Text = "C:/ABC/Winecrash";
            // 
            // _buttonInstall
            // 
            this._buttonInstall.Location = new System.Drawing.Point(182, 432);
            this._buttonInstall.Name = "_buttonInstall";
            this._buttonInstall.Size = new System.Drawing.Size(122, 29);
            this._buttonInstall.TabIndex = 3;
            this._buttonInstall.Text = "Download and Install";
            this._buttonInstall.UseVisualStyleBackColor = true;
            this._buttonInstall.Click += new System.EventHandler(this._buttonInstall_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Enabled = false;
            this._cancelButton.Location = new System.Drawing.Point(310, 432);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(122, 29);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _installProgress
            // 
            this._installProgress.Location = new System.Drawing.Point(12, 400);
            this._installProgress.Name = "_installProgress";
            this._installProgress.Size = new System.Drawing.Size(420, 26);
            this._installProgress.TabIndex = 5;
            // 
            // _createDesktopShortcut
            // 
            this._createDesktopShortcut.Checked = true;
            this._createDesktopShortcut.CheckState = System.Windows.Forms.CheckState.Checked;
            this._createDesktopShortcut.Location = new System.Drawing.Point(12, 304);
            this._createDesktopShortcut.Name = "_createDesktopShortcut";
            this._createDesktopShortcut.Size = new System.Drawing.Size(151, 24);
            this._createDesktopShortcut.TabIndex = 6;
            this._createDesktopShortcut.Text = "Create a desktop shortcut";
            this._createDesktopShortcut.UseVisualStyleBackColor = true;
            // 
            // _createStartShortcut
            // 
            this._createStartShortcut.Checked = true;
            this._createStartShortcut.CheckState = System.Windows.Forms.CheckState.Checked;
            this._createStartShortcut.Location = new System.Drawing.Point(12, 323);
            this._createStartShortcut.Name = "_createStartShortcut";
            this._createStartShortcut.Size = new System.Drawing.Size(186, 24);
            this._createStartShortcut.TabIndex = 7;
            this._createStartShortcut.Text = "Create a start menu shortcut";
            this._createStartShortcut.UseVisualStyleBackColor = true;
            // 
            // _openInstall
            // 
            this._openInstall.Checked = true;
            this._openInstall.CheckState = System.Windows.Forms.CheckState.Checked;
            this._openInstall.Location = new System.Drawing.Point(12, 353);
            this._openInstall.Name = "_openInstall";
            this._openInstall.Size = new System.Drawing.Size(186, 24);
            this._openInstall.TabIndex = 8;
            this._openInstall.Text = "Open launcher when installed";
            this._openInstall.UseVisualStyleBackColor = true;
            // 
            // _status
            // 
            this._status.Location = new System.Drawing.Point(12, 440);
            this._status.Name = "_status";
            this._status.Size = new System.Drawing.Size(164, 33);
            this._status.TabIndex = 9;
            this._status.Text = "Status";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(420, 102);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // _welcome
            // 
            this._welcome.Location = new System.Drawing.Point(12, 127);
            this._welcome.Name = "_welcome";
            this._welcome.Size = new System.Drawing.Size(422, 99);
            this._welcome.TabIndex = 11;
            this._welcome.Text = resources.GetString("_welcome.Text");
            // 
            // Installer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 473);
            this.Controls.Add(this._welcome);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this._status);
            this.Controls.Add(this._openInstall);
            this.Controls.Add(this._createStartShortcut);
            this.Controls.Add(this._createDesktopShortcut);
            this.Controls.Add(this._installProgress);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._buttonInstall);
            this.Controls.Add(this._actualInstallationLabel);
            this.Controls.Add(this._installLabel);
            this.Controls.Add(this._pathInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Installer";
            this.Text = "Winecrash Installer";
            this.Load += new System.EventHandler(this.Installer_Load);
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label _welcome;

        private System.Windows.Forms.PictureBox pictureBox1;

        private System.Windows.Forms.Label _status;

        private System.Windows.Forms.CheckBox _createDesktopShortcut;
        private System.Windows.Forms.CheckBox _createStartShortcut;
        private System.Windows.Forms.CheckBox _openInstall;

        private System.Windows.Forms.ProgressBar _installProgress;

        private System.Windows.Forms.Button _cancelButton;

        private System.Windows.Forms.Button _buttonInstall;

        private System.Windows.Forms.Label _actualInstallationLabel;
        private System.Windows.Forms.Label _installLabel;

        private System.Windows.Forms.TextBox _pathInput;

        #endregion
    }
}