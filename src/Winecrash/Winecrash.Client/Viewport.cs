using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private void Viewport_Load(object sender, EventArgs e)
        {
            this.FormClosing += Viewport_FormClosing;

            Engine.Engine.Load();

            Updater.OnFrameStart += Debug;

            
        }

        private void Viewport_FormClosing(object sender, FormClosingEventArgs e)
        {
            Engine.Engine.Stop();
        }

        private void Debug()
        {
            InvokeOnMainThread(() =>
            {
                lb_debug.Text = "Time since beginning: " + Math.Round(Time.TimeSinceStart,4) + " secs\n with a current delta of " + Math.Round(Time.DeltaTime, 4) + " secs\n(" + Math.Round((1D/ Time.DeltaTime)) + "FPS)";

                Vector2D v2 = new Vector2D(2.0D);
                Vector3D v3 = new Vector3D(v2, 8.98D);

                v3.YZ *= 5.0F;

                Vector3D v3bis = new Vector3D(5.0D);

                v3 /= v3bis;

                VectorND vn = new VectorND(10, 1.0D);

                Vector4D d = new Vector4D();

                vn[5] = 5.0D;

                double angle = Time.TimeSinceStart * 20.0D;
                Quaternion quat = new Quaternion((Vector3D.Forward) * angle);


                lb_debug.Text += "\n\n\nQuaternion: " + quat.ToString() + "\nEuler: " + quat.Euler.ToString();// + "\nDirection: " + quat.Direction;
            });
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
