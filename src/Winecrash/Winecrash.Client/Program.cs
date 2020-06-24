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
            wobj1.AddModule<DebugHelp>();

            WObject wobj2 = new WObject("Child Test Object");

            WObject wobj3 = new WObject("Mom Test Object");

            wobj1.Position = new Vector3F(10.0F, 0.0F, 0.0F);
            wobj1.Scale = new Vector3F(0.5F);

            wobj2.Parent = wobj1;

            wobj1.Rotation = new Quaternion(0, 90, 0);
            wobj1.Scale = new Vector3F(1.0F);
            wobj1.Position += Vector3F.One * 10.0F;

            //wobj1.Scale = new Vector3F(0.5F);//Rotation = new Quaternion(0.0F, 90.0F, 0.0F);
            //wobj2.Position = new Vector3F(10.0F, 0.0F, 0.0F);

            Debug.Log("Local / Global");
            Debug.Log("Position: " + wobj2.LocalPosition + " : " + wobj2.Position);
            Debug.Log("Rotation: " + wobj2.LocalRotation.Euler + " : " + wobj2.Rotation.Euler);
            Debug.Log("Scale: " + wobj2.LocalScale + " : " + wobj2.Scale);

            //Mesh[] meshes = Mesh.LoadFile("assets/models/cube.obj", MeshFormats.Wavefront);

            /*if (meshes != null)
                for (int i = 0; i < meshes.Length; i++)
                    Debug.Log(meshes[i].Name);*/

            /*MeshRenderer mr = wobj1.AddModule<MeshRenderer>();
            if (meshes != null && meshes.Length > 0)
                mr.Mesh = meshes[0];*/


            //Thread.Sleep(500);

            //mr.Delete();
            //mr.Mesh.Delete();

            //WEngine.TraceLayers();

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
