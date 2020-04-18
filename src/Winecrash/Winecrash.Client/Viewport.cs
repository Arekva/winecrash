using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winecrash.Engine;

namespace Winecrash.Client
{
    public partial class Viewport : Form
    {
        public static Viewport Instance { get; private set; }

        public Viewport()
        {
            if (Instance != null) this.Dispose(true);

            Instance = this;
            InitializeComponent();
        }

        private static Image img;
        private void Viewport_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.FormClosing += Viewport_FormClosing;
            this.SizeChanged += Viewport_SizeChanged;
            
            this.OnSizeChanged(EventArgs.Empty);

            Engine.Engine.Load();
            
            Render.OnFrameRendered += OnFrameRender;
        }

        private void Viewport_SizeChanged(object sender, EventArgs e)
        {
            Render.SetNextFrameResolution(this.Size.Width, this.Size.Height);
            this.ViewRender.Size = new Size(this.Size.Width, this.Size.Height);
        }

        private void Viewport_FormClosing(object sender, FormClosingEventArgs e)
        {
            Engine.Engine.Stop();
        }
        
        private static double drawTime = 0;
        private void OnFrameRender(RenderImage renderImage)
        {
            double t = Time.TimeSinceStart;
            InvokeOnMainThread(() => { this.ViewRender.Image = renderImage.Image; });
            drawTime = Time.TimeSinceStart - t;
            
            Debug.Log("Draw time: " + Math.Round(1000D * drawTime, 1) + "ms");
        }

        public static void InvokeOnMainThread(Action action)
        {
            try
            {
                Viewport.Instance?.Invoke(action);
            }
            catch { }
        }
    }
}
