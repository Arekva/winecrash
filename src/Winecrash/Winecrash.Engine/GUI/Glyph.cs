using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Winecrash.Engine.GUI
{
    public struct Glyph
    {
        /// <summary>
        /// The character represented by the glyph
        /// </summary>
        public char Character { get; }

        /// <summary>
        /// The X (horizontal) position of the glyph onto the texture map.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The Y (vertical) position of the glyph onto the texture map.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The width of the glyph onto the texture map.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the glyph onto the texture map.
        /// </summary>
        public int Height { get; set; }

        public Glyph(char character)
        {
            this.Character = character;

            this.X = this.Y = 0;
            this.Width = this.Height = 1024;
        }

        public Glyph(char character, Vector2I position, Vector2I size)
        {
            this.Character = character;
            this.X = position.X;
            this.Y = position.Y;
            this.Width = size.X;
            this.Height = size.Y;
        }

        [JsonConstructor]
        public Glyph(char character, int x, int y, int width, int height)
        {
            this.Character = character;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
    }
}
