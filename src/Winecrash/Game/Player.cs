using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;

namespace Winecrash
{
    public class Player : BaseObject
    {
        public string Nickname { get; private set; }
        public Guid GUID { get; private set; }
        public Texture Skin { get; private set; }

        public Player(string nickname/*, Guid guid, string skinAddress = null*/)
        {
            this.Nickname = nickname;
        }

        public override void Delete()
        {
            this.Nickname = null;
            if(Engine.DoGUI)
                this.Skin?.Delete();
            base.Delete();
        }
    }
}
