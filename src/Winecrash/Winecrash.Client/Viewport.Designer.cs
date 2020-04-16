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
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_debug = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lb_debug
            // 
            this.lb_debug.AutoSize = true;
            this.lb_debug.Location = new System.Drawing.Point(12, 9);
            this.lb_debug.Name = "lb_debug";
            this.lb_debug.Size = new System.Drawing.Size(51, 13);
            this.lb_debug.TabIndex = 0;
            this.lb_debug.Text = "lb_debug";
            // 
            // Viewport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lb_debug);
            this.Name = "Viewport";
            this.Text = "Winecrash Viewport";
            this.Load += new System.EventHandler(this.Viewport_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_debug;
    }
}

