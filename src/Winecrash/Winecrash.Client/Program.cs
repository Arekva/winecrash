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
            WObject wobj1 = new WObject("Test Object");

            Mesh[] meshes = Mesh.LoadFile("Models/Cube.obj", MeshFormats.Wavefront);

            /*if (meshes != null)
                for (int i = 0; i < meshes.Length; i++)
                    Debug.Log(meshes[i].Name);*/

            MeshRenderer mr = wobj1.AddModule<MeshRenderer>();
            if (meshes != null && meshes.Length > 0)
                mr.Mesh = meshes[0];

            if(mr.Mesh != null)
            {
                Debug.Log("= V3F Vertices =");
                for (int i = 0; i < mr.Mesh.Vertices.Length; i++)
                {
                    Debug.Log(mr.Mesh.Vertices[i]);
                }

                Debug.Log("\n= Float Vertices =");

                float[] floats = mr.Mesh.VerticesFloatArray();

                for (int i = 0; i < floats.Length; i+=3)
                {
                    Debug.Log(floats[i] + " " + floats[i + 1] + " " + floats[i + 2]);
                }
            }

            //Thread.Sleep(500);

            //mr.Delete();
            //mr.Mesh.Delete();

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
