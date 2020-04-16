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
            Engine.Engine.Load();

            Updater.OnFrameStart += Debug;

            
        }

        private void Debug()
        {
            InvokeOnMainThread(() =>
            {
                string msg = "A ";
                if(Input.IsPressed(Keys.A))
                {
                    msg += " pressed";
                }
                else if (Input.IsPressing(Keys.A))
                {
                    msg += " pressing";
                }
                else if(Input.IsReleased(Keys.A))
                {
                    //msg += " released";
                }
                else if (Input.IsReleasing(Keys.A))
                {
                    msg += "releasing";
                }
                if (msg != "A ")
                    MessageBox.Show(msg);
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
