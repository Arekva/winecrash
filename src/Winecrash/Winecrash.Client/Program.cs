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

            WObject wobj1 = new WObject("Test Object #1");
            TestModule mod1 = wobj1.AddModule<TestModule>();
            mod1.Name = "TestModule #1";

            WObject wobj2 = new WObject("Test Object #2");
            TestModule mod2 = wobj2.AddModule<TestModule>();
            mod2.Name = "TestModule #2";


            WEngine.TraceLayers();

            mod1.Group = int.MinValue;
            mod2.Group = 1;

            Group.SetGroupLayer(int.MinValue, -10);
            Group.SetGroupLayer(1, 1);
            Group.SetGroupLayer(0, 1);

            

            WEngine.TraceLayers();

            Thread.Sleep(1000);
            mod1.Group = mod2.Group = 0;
            mod1.ExecutionOrder = 10;
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
