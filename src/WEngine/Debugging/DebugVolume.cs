using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEngine.Debugging
{
    public class DebugVolume : Module
    {
        private Color256 _OutlineColor = WEngine.Color256.Green;
        public Color256 OutlineColor
        {
            get
            {
                return _OutlineColor;
            }

            set
            {
                _OutlineColor = value;
                
                Renderer?.Material.SetData("outlineColor", OutlineColor);
            }
        }
        
        public Color256 _FillColor = Color256.Transparent;

        public Color256 FillColor
        {
            get
            {
                return _FillColor;
            }

            set
            {
                _FillColor = value;
                
                Renderer?.Material.SetData("fillColor", FillColor);
            }
        }

        protected MeshRenderer Renderer = null;

        protected static Mesh VolumeMesh = null;

        static DebugVolume()
        {
            VolumeMesh = Mesh.LoadFile("assets/models/DebugVolume.obj", MeshFormats.Wavefront);
            
        }
        

        protected internal override void Creation()
        {
            Renderer = this.WObject.AddModule<MeshRenderer>();
            Renderer.DrawOrder = 99999;
            Renderer.Culling = Culling.Front;

            Renderer.Mesh = VolumeMesh;
            
            Renderer.Material = new Material(Shader.Find("DebugVolume"));
            Renderer.Material.SetData("outlineColor", OutlineColor);
            Renderer.Material.SetData("fillColor", FillColor);
            
        }

        protected internal override void OnDelete()
        {
            Renderer?.Delete();
            Renderer = null;
        }
    }
}
