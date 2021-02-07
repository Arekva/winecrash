using System.Collections.Generic;
using WEngine.Debugging;
using WEngine.GUI;

namespace WEngine
{
    public class UIBoxCollider : BoxCollider
    {
        public GUIModule GUIModule { get; set; }

        protected internal override void Creation()
        {
            GUIModule = this.WObject.GetModule<GUIModule>();
            base.Creation();
        }

        public override Vector3D Center => this.GUIModule.GlobalPosition + Offset;
    }
}
