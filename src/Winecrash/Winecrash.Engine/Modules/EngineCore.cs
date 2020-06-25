using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public class EngineCore : Module
    {
        public static EngineCore Instance { get; private set; }

        public override bool Undeletable { get; internal set; } = true;

        protected internal override void Creation()
        {
            if(Instance)
            {
                this.ForcedDelete();
                return;
            }

            Instance = this;
        }

        protected internal override void Update()
        {
            // alt f4 close
            if (Input.IsPressed(Keys.LeftAlt) && Input.IsPressed(Keys.F4))
            {
                Viewport.Instance.Close();
                return;
            }

            // fullscreen
            if (Input.IsPressed(Keys.LeftAlt) && Input.IsPressed(Keys.Enter))
            {
                throw new NotImplementedException("Fullscreen switch not done yet.");
            }
        }

        protected internal override void OnDelete()
        {
            if(Instance && Instance == this)
            {
                Instance = null;
            }
        }
    }
}
