using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winecrash.Engine;

namespace Winecrash.Client
{
    static class Program
    {
        static void Main()
        {
            WEngine.Run();

            CreateDebugWindow();
            //MessageBox.Show("Exiting program");
        }

        [STAThread]
        static void CreateDebugWindow()
        {
            WEngine.OnStop += () => Application.Exit();

            Debug.AddLogger(new Logger(frmDebug.Log));

            Application.Run(new frmDebug());
        }
    }
}
