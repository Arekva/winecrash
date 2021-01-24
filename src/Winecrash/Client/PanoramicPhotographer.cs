using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using WEngine;
using Winecrash;
using Graphics = WEngine.Graphics;

namespace Winecrash.Client
{
    public class PanoramicPhotographer : Module
    {
        public Vector2I Resolution { get; set; } = Vector2I.One * 1024;
        public string SaveDirectory { get; set; } = Path.Combine(Folders.UserData, "Panoramas");
        public double FOV { get; set; } = 90.0D;

        public const double Offset = 45.0D;

        public CursorLockModes LockMode { get; set; } = CursorLockModes.Free;

        public Dictionary<string, Quaternion> FaceDirections { get; set; } = new Dictionary<string, Quaternion>(6)
        {
            {"-z", new Quaternion(0,Offset,0)},
            {"+z", new Quaternion(0,180+Offset,0)},
            {"+y", new Quaternion(-90,180+Offset,0)},
            {"-y", new Quaternion(90,180+Offset,0)},
            {"+x", new Quaternion(0,90+Offset,0)},
            {"-x", new Quaternion(0,-90+Offset,0)},
        };
        protected override void Update()
        {
            if (Input.IsPressed(Keys.LeftControl) && Input.IsPressing(Keys.F11))
            {
                TakePanorama();
            }
        }

        public void TakePanorama()
        {
            Task.Run(async () =>
            {
                Camera camera = Camera.Main;
                WObject camWobj = camera.WObject;
                IWindow window = Graphics.Window;
                Vector2I baseRes = window.SurfaceFixedResolution;
                Quaternion baseRot = camWobj.Rotation;
                double baseFov = camera.FOV;
                CursorLockModes baseLock = Input.LockMode;

                PlayerController.CameraLocked = true;
                
                Input.LockMode = LockMode;
                camera.FOV = FOV;
                window.SurfaceFixedResolution = Resolution;

                string finalFolder = Path.Combine(SaveDirectory, Time.ShortTimeForFile);

                Directory.CreateDirectory(finalFolder);

                foreach (var kvp in FaceDirections)
                {
                    string face = kvp.Key;
                    Quaternion dir = kvp.Value;

                    camWobj.Rotation = dir;

                    await Task.Delay(250);
                    /*
                    await window.WaitForNextFrame();*/

                    using (Bitmap map = window.Screenshot())
                        map.Save(Path.Combine(finalFolder, face + ".png"));
                }

                PlayerController.CameraLocked = false;
                window.SurfaceFixedResolution = baseRes;
                camWobj.Rotation = baseRot;
                camera.FOV = baseFov;
                Input.LockMode = baseLock;
            });
        }
    }
}