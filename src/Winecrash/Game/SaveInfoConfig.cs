using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;

namespace Winecrash
{
    public class SaveInfoConfig : Config
    {
        private string _Name = Winecrash.Save.DefaultName;
        /// <summary>
        /// The name of the save.
        /// </summary>
        [Synchronize]
        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                this._Name = value;
                FireConfigChanged();
            }
        }

        private WEngine.Version _Version = Game.Version;
        /// <summary>
        /// The version of the save.
        /// </summary>
        [Synchronize]
        public WEngine.Version Version
        {
            get
            {
                return _Version;
            }

            set
            {
                _Version = value;
                FireConfigChanged();
            }
        }

        public SaveInfoConfig(string path) : base(path) { }
    }
}
