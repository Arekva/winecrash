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

        public Player(string nickname)
        {
            this.Nickname = nickname;
        }

        public override void Delete()
        {
            this.Nickname = null;
            base.Delete();
        }
    }
}
