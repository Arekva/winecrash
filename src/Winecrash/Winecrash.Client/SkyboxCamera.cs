using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Client
{
    class SkyboxCamera : Module
    {
        public Camera ReferenceCamera { get; set; }
        public Camera SkyCam { get; private set; }

        protected override void Creation()
        {
            SkyCam = this.WObject.AddModule<Camera>();
            SkyCam.NearClip = 0.1F;
            SkyCam.FarClip = 1.1F;
            SkyCam.Depth = -1000;
        }

        protected override void Update()
        {
            if(ReferenceCamera != null)
            {
                SkyCam.FOV = ReferenceCamera.FOV;
                SkyCam.WObject.Rotation = ReferenceCamera.WObject.Rotation;
            }
        }

        protected override void OnDelete()
        {
            ReferenceCamera = null;
            SkyCam = null;
        }
    }
}
