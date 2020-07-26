using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;
using Newtonsoft.Json;
using System.IO;

namespace Winecrash.Game
{
    [Serializable]
    public class Structure : BaseObject
    {
        private List<Structure> _Cache = new List<Structure>();
        public Vector3I Size { get; set; }
        public Vector3I Root { get; set; }

        public Structure Get(string name)
        {
            return _Cache.FirstOrDefault(str => str.Name == name);
        }

        public ushort this[int index]
        {
            get
            {
                return _Blocks[index];
            }

            set
            {
                _Blocks[index] = value;
            }
        }

        public ushort this[int x, int y, int z]
        {
            get
            {
                return GetBlockIndex(x, y, z);
            }

            set
            {
                SetBlock(x, y, z, value);
            }
        }

        private ushort[] _Blocks = null;

        public static Structure Load(string path)
        {
            Structure str = null;

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = File.OpenText(path))
            using (JsonTextReader jtr = new JsonTextReader(sr))
            {
                str = (Structure)serializer.Deserialize(jtr, typeof(Structure));
            }

            return str;
        }

        [JsonConstructor]
        public Structure(string name, Vector3I size, Vector3I root, string[] palette, int[] data) : base(name)
        {
            this.Size = size;
            this.Root = root;

            ushort[] paletteIDs = new ushort[palette.Length];
            _Blocks = new ushort[data.Length];

            //create palette ids
            for (int i = 0; i < palette.Length; i++)
            {
                paletteIDs[i] = ItemCache.GetIndex(palette[i]);
            }

            //create blocks
            for (int i = 0; i < data.Length; i++)
            {
                _Blocks[i] = paletteIDs[data[i]];
            }

            _Cache.Add(this);
        }

        private ushort GetBlockIndex(int x, int y, int z)
        {
            return this._Blocks[x + this.Size.X * y + this.Size.X * this.Size.Y * z];
        }

        private void SetBlock(int x, int y, int z, ushort cacheIndex)
        {
            this._Blocks[x + this.Size.X * y + this.Size.X * this.Size.Y * z] = cacheIndex;
        }

        public override void Delete()
        {
            _Blocks = null;
            _Cache.Remove(this);
            base.Delete();
        }
    }
}
