using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Game
{
    class SkyboxCamera : Module
    {
        public Camera ReferenceCamera { get; set; }
        public Camera SkyCam { get; private set; }
        public SkyboxCamera Instance { get; private set; }

        protected override void Creation()
        {
            if(Instance != null)
            {
                this.Delete();
                return;
            }
            Instance = this;
            
            SkyCam = this.WObject.AddModule<Camera>();
            SkyCam._NearClip = 0.1F;
            SkyCam._FarClip = 1.1F;
            SkyCam.Depth = -1000;
            SkyCam.RenderLayers = 1L << 32;
            SkyCam.Name = "Skybox Camera";
        }

        protected override void LateUpdate()
        {
            if (ReferenceCamera != null)
            {
                SkyCam.FOV = ReferenceCamera.FOV;
                SkyCam.WObject.LocalRotation = ReferenceCamera.WObject.LocalRotation;
            }
        }
        protected override void OnRender()
        {
            base.OnRender();
        }

        protected override void OnDelete()
        {
            ReferenceCamera = null;
            SkyCam = null;
        }
    }
}
