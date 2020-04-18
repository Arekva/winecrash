namespace Winecrash.Client
{
    partial class Viewport
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ViewRender = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize) (this.ViewRender)).BeginInit();
            this.SuspendLayout();
            // 
            // ViewRender
            // 
            this.ViewRender.BackColor = System.Drawing.Color.Black;
            this.ViewRender.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ViewRender.Location = new System.Drawing.Point(0, 0);
            this.ViewRender.Name = "ViewRender";
            this.ViewRender.Size = new System.Drawing.Size(617, 357);
            this.ViewRender.TabIndex = 0;
            this.ViewRender.TabStop = false;
            // 
            // Viewport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 519);
            this.Controls.Add(this.ViewRender);
            this.Name = "Viewport";
            this.Text = "Winecrash Viewport";
            this.Load += new System.EventHandler(this.Viewport_Load);
            ((System.ComponentModel.ISupportInitialize) (this.ViewRender)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox ViewRender;
    }
}

