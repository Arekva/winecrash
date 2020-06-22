using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winecrash.Engine;
using System.Threading;

namespace Winecrash.Client
{
    static class Program
    {
        static void Main()
        {
            

            

            Task.Run(CreateDebugWindow);

            WEngine.Run();

            System.Threading.Thread.Sleep(500);

            WObject wobj1 = new WObject("Test Object 1");
            wobj1.AddModule<TestModule>();

            WObject wobj2 = new WObject("Test Object 2");
            TestModule mod2 = wobj2.AddModule<TestModule>();
            //mod2.Group = 1;

            //Debug.Log(Group.GetGroup(1).Name);

            WEngine.TraceLayers();

            wobj1.Delete();
            wobj1 = null;

            Thread.Sleep(2000);

            wobj2.Delete();

            WEngine.TraceLayers();
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
