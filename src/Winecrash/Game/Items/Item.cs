using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WEngine;

namespace Winecrash
{
    public abstract class Item : IEquatable<Item>
    {
        //-- set in database config --//
        [JsonIgnore]
        public string Identifier { get; internal set; } = "winecrash:air";
        
        //-- set in item config --//
        public string TexturePath { get; set; } = null;
        
        [JsonIgnore]
        public Mesh Model { get; set; } = null;
        [JsonIgnore]
        public Texture Texture { get; set; } = null;


        public virtual void OnDeserialize()
        {
            if(!string.IsNullOrEmpty(TexturePath))
                Texture = WEngine.Texture.GetOrCreate(TexturePath);
        }

        internal virtual Mesh BuildModel()
        {
            uint index = ItemCache.GetIndex(this.Identifier);
            
            //by default, create a 3D version of the image
            uint texSize = ItemCache.TextureSize;

            // min / max UV coordinates for the texture
            double minXPos = 0;
            double maxXPos = 1.0D / 6.0D;
            double minYPos = index / (double)ItemCache.TotalItems;
            double maxYPos = (index+1) / (double)ItemCache.TotalItems;

            List<Vector3F> vertices = new List<Vector3F>();
            List<Vector2F> uvs = new List<Vector2F>();
            List<Vector3F> normals = new List<Vector3F>();
            List<uint> triangles = new List<uint>();
            

            for (int y = 0; y < texSize; y++)
            {
                for (int x = 0; x < texSize; x++)
                {
                    
                }
            }

            return null;
        }

        public bool Equals(Item other)
        {
            return other != null && other.Identifier == this.Identifier;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Item);
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        public void Delete()
        {
            this.Model?.Delete();
        }

        public override string ToString()
        {
            return $"{this.Identifier} ({this.GetType().Name})";
        }
    }
}
