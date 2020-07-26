using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Game
{
    public class Cube : Block
    {
        public TexturePaths Textures { get; set; }

        public Texture UpTexture { get; private set; }
        public Texture DownTexture { get; private set; }

        public Texture EastTexture { get; private set; }
        public Texture WestTexture { get; private set; }

        public Texture NorthTexture { get; private set; }
        public Texture SouthTexture { get; private set; }

        public override void OnDeserialize()
        {
            UpTexture = String.IsNullOrEmpty(Textures.Up) ? Texture.Blank : Texture.GetOrCreate(Textures.Up);
            DownTexture = String.IsNullOrEmpty(Textures.Down) ? Texture.Blank : Texture.GetOrCreate(Textures.Down);

            EastTexture = String.IsNullOrEmpty(Textures.East) ? Texture.Blank : Texture.GetOrCreate(Textures.East);
            WestTexture = String.IsNullOrEmpty(Textures.West) ? Texture.Blank : Texture.GetOrCreate(Textures.West);

            NorthTexture = String.IsNullOrEmpty(Textures.North) ? Texture.Blank : Texture.GetOrCreate(Textures.North);
            SouthTexture = String.IsNullOrEmpty(Textures.South) ? Texture.Blank : Texture.GetOrCreate(Textures.South);

            base.OnDeserialize();
        }
    }
}
