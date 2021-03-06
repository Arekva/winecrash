﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;

namespace Winecrash
{
    public class SaveInfoConfig : Config
    {
        public const string RelativePath = "info.json";

        private string _Name = "world";
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

        private WEngine.Version _Version = Winecrash.Version;
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
