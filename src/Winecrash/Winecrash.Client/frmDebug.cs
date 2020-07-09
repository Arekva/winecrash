using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winecrash.Client
{
    public partial class frmDebug : Form
    {
        public static frmDebug Instance { get; private set; }
        public frmDebug()
        {
            InitializeComponent();
        }

        private void frmDebug_Load(object sender, EventArgs e)
        {
            Instance = this;
            this.Resize += FrmDebug_Resize;
        }

        private void FrmDebug_Resize(object sender, EventArgs e)
        {
            this.richTextBox1.Size = this.Size;
        }

        public static void InvokeMain(Action a)
        {
            if (Instance == null || Instance.IsDisposed) return;
            try
            {
                Instance.Invoke(a);
            }
            catch { }
            
        }

        public static void Log(object obj)
        {
            /*Task.Run(() =>
            {*/
                InvokeMain(() =>
                {
                    Instance.richTextBox1.Text += $"{obj}\n";

                    Instance.richTextBox1.Select(Instance.richTextBox1.Text.Length - 1, 0);
                    Instance.richTextBox1.ScrollToCaret();
                });
            //});
        }
    }
}
