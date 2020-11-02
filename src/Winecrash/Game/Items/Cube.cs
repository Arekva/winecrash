using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WEngine;

namespace Winecrash
{
    public class Cube : Block
    {
        //-- set in item config --//
        public CubeTexturePaths Textures { get; set; }

        [JsonIgnore]
        public Texture UpTexture { get; private set; }
        [JsonIgnore]
        public Texture DownTexture { get; private set; }
        [JsonIgnore]
        public Texture EastTexture { get; private set; }
        [JsonIgnore]
        public Texture WestTexture { get; private set; }
        [JsonIgnore]
        public Texture NorthTexture { get; private set; }
        [JsonIgnore]
        public Texture SouthTexture { get; private set; }

        public override void OnDeserialize()
        {
            if (Engine.DoGUI)
            {
                UpTexture = String.IsNullOrEmpty(Textures.Up) ? Texture.Blank : Texture.GetOrCreate(Textures.Up);
                DownTexture = String.IsNullOrEmpty(Textures.Down) ? Texture.Blank : Texture.GetOrCreate(Textures.Down);

                EastTexture = String.IsNullOrEmpty(Textures.East) ? Texture.Blank : Texture.GetOrCreate(Textures.East);
                WestTexture = String.IsNullOrEmpty(Textures.West) ? Texture.Blank : Texture.GetOrCreate(Textures.West);

                NorthTexture = String.IsNullOrEmpty(Textures.North)
                    ? Texture.Blank
                    : Texture.GetOrCreate(Textures.North);
                SouthTexture = String.IsNullOrEmpty(Textures.South)
                    ? Texture.Blank
                    : Texture.GetOrCreate(Textures.South);
            }
            base.OnDeserialize();
            
        }
    }
}