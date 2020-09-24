﻿using Newtonsoft.Json;
using WEngine;
using WEngine.Networking;

namespace Winecrash.Net
{
    public class NetChunk : NetObject
    {
        public Vector2I Coordinates { get; }
        public string Dimension { get; }
        public string[] Palette { get; }
        public ushort[] Indices { get; }
        

        public NetChunk(Chunk chunk) : this(chunk.ToSave()) { }

        public NetChunk(SaveChunk saveChunk)
        {
            this.Coordinates = saveChunk.Coordinates;
            this.Dimension = saveChunk.Dimension;
            this.Palette = saveChunk.Palette;
            this.Indices = saveChunk.Indices;
        }

        [JsonConstructor]
        public NetChunk(Vector2I coordinates, string dimension, string[] palette, ushort[] indices)
        {
            this.Coordinates = coordinates;
            this.Dimension = dimension;
            this.Palette = palette;
            this.Indices = indices;
        }

        public SaveChunk ToSave()
        {
            return new SaveChunk()
            {
                Palette = Palette,
                Indices = Indices,
                Coordinates = Coordinates,
                Dimension = Dimension
            };
        }
    }
}