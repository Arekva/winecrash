using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Client
{
    [Serializable]
    public class DictionnaryChunk
    {
        public string[] Dictionnary
        {
            get
            {
                return _dictionnary;
            }

            set
            {
                _dictionnary = value;
            }
        }
        private string[] _dictionnary;

        public int[] References
        {
            get
            {
                return _references;
            }

            set
            {
                _references = value;
            }
        }
        private int[] _references;

        [JsonConstructor]
        public DictionnaryChunk(string[] dictionnary, int[] references)
        {
            this._dictionnary = dictionnary;
            this._references = references;
        }
    }
}
