using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

using System.Windows.Forms;

namespace Winecrash.Engine
{
    public sealed class MeshRenderer : Module
    {
        private Mesh _Mesh = null;
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }

        internal static List<MeshRenderer> ActiveMeshRenderers = new List<MeshRenderer>();

        internal void Use(Matrix4 transform)
        {
            //if (_Mesh == null || _Mesh.Vertices == null || _Mesh.Triangles == null) return;

            if(Material == null)
            {
                Shader.ErrorShader.Use();
                Shader.ErrorShader.SetMatrix4("transform", transform);
            }

            else
            {
                Material.SetData<Matrix4>("transform", transform);
                Material.Use();
            }

            //GL.BindVertexArray()
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            //GL.DrawElements(PrimitiveType.Triangles, _Mesh.Triangles.Length, DrawElementsType.UnsignedInt, 0);
        }

        protected internal override void Creation()
        {
            ActiveMeshRenderers.Add(this);
            this.Group = -1;
        }
        protected internal override void Update()
        {

        }
        protected internal override void OnEnable()
        {
            ActiveMeshRenderers.Add(this);
        }

        protected internal override void OnDisable()
        {
            ActiveMeshRenderers.Remove(this);
        }

        protected internal override void OnDelete()
        {
            ActiveMeshRenderers.Remove(this);
            this._Mesh = null;
        }
    }
}
