using System;

using Newtonsoft.Json;

namespace WEngine.GUI
{
    public struct Glyph : IEquatable<Glyph>
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

        public override bool Equals(object obj)
        {
            return obj is Glyph g ? Equals(g) : false;
        }

        public bool Equals(Glyph g)
        {
            return g.X == this.X && g.Y == this.Y && g.Character == this.Character && g.Width == this.Width && g.Height == this.Height;
        }

        public override int GetHashCode()
        {
            var hashCode = 175765292;
            hashCode = hashCode * -1521134295 + Character.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"Glyph['{Character}']";
        }

        public static bool operator ==(Glyph a, Glyph b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Glyph a, Glyph b)
        {
            return !(b == a);
        }
    }
}
