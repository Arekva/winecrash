using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace Winecrash.Engine
{
    public sealed class EngineCore : Module
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

        protected internal override void Start()
        {
            this.WObject.Position = Vector3F.Left * 5.0F;
            
            
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
            if (Input.IsPressed(Keys.LeftAlt) && Input.IsPressed(Keys.Enter) || Input.IsPressed(Keys.F11))
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
