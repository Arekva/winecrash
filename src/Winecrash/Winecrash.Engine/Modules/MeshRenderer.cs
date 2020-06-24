using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public sealed class MeshRenderer : Module
    {
        //public Int64[] dummy = new long[50_000_000];

        private Mesh _Mesh = null;
        public Mesh Mesh
        {
            get
            {
                return this._Mesh;
            }

            set
            {
                if(this.Enabled)
                {
                    ActiveMeshes.Add(value);
                }

                _Mesh = value;
            }
        }

        internal static List<Mesh> ActiveMeshes = new List<Mesh>();

        protected internal override void Creation()
        {
            this.Group = -1;
        }
        protected internal override void Update()
        {
            // Manage mesh references in case of deletion
            if(_Mesh != null && _Mesh.Deleted)
            {
                ActiveMeshes.Remove(_Mesh);
                this._Mesh = null;
            }
        }
        protected internal override void OnEnable()
        {
            if(_Mesh != null)
                ActiveMeshes.Add(this._Mesh);
        }

        protected internal override void OnDisable()
        {
            if (_Mesh != null)
                ActiveMeshes.Remove(this._Mesh);
        }

        protected internal override void OnDelete()
        {
            if (_Mesh != null)
                ActiveMeshes.Remove(this._Mesh);
            this._Mesh = null;
        }
    }
}
