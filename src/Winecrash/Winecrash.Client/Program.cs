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

            Thread.Sleep(1000);
            WObject wobj1 = new WObject("Test Object #1");
            wobj1.AddModule<TestModule>().Name = "Test Module #1";

            Mesh[] meshes = Mesh.LoadFile("Models/Cube.obj", MeshFormats.Wavefront);

            if (meshes != null)
                for (int i = 0; i < meshes.Length; i++)
                    Debug.Log(meshes[i].Name);

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
