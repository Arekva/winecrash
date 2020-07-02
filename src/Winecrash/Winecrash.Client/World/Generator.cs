using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibNoise;

namespace Winecrash.Client
{
    public static class Generator
    {
        public static ushort[] GetChunk(int x, int y, out bool generated)
        {
            string fileName = "save/" + $"c{x}_{y}.json";

            if (File.Exists(fileName))
            {
                generated = false;
                return LoadFromSave(fileName);
            }
            else
            {
                generated = true;
                return Generate(x, y);
            }

        }

        static LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin("lol".GetHashCode(), NoiseQuality.Standard);
        private static ushort[] Generate(int chunkx, int chunky)
        {
            ushort[] blocks = new ushort[Chunk.Width * Chunk.Height * Chunk.Depth];
            
            for (int z = 0; z < Chunk.Depth; z++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        string id = "winecrash:air";

                        const float scale = 0.025F;
                        const float shiftX = 10000;
                        const float shiftZ = 10000;

                        int height = (int)(perlin.GetValue((chunkx * Chunk.Width + shiftX + x) * scale, (chunky * Chunk.Depth + shiftZ + z) * scale) * 5) + 60;

                        if(y == height)
                        {
                            id = "winecrash:grass";
                        }
                        else if(y < height)
                        {
                            if(y > height - 3)
                                id = "winecrash:dirt";
                            else
                                id = "winecrash:stone";
                        }

                        if (y == 2)
                        {
                            if (World.WorldRandom.NextDouble() < 0.33D)
                            {
                                id = "winecrash:bedrock";
                            }
                        }
                        else if (y == 1)
                        {
                            if (World.WorldRandom.NextDouble() < 0.66D)
                            {
                                id = "winecrash:bedrock";
                            }
                        }
                        else if (y == 0)
                        {
                            id = "winecrash:bedrock";
                        }


                        //Server.Log(id);
                        blocks[x + Chunk.Width * y + Chunk.Width * Chunk.Height * z] = ItemCache.GetIndex(id);//new Block(id);
                    }
                }
            }

            return blocks;
        }
        private static ushort[] LoadFromSave(string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = File.OpenText(path))
            using (JsonTextReader jtr = new JsonTextReader(sr))
            {
                JSONChunk dc = (JSONChunk)serializer.Deserialize(jtr, typeof(JSONChunk));

                ushort[] blocks = new ushort[Chunk.Width * Chunk.Height * Chunk.Depth];
                int chunkindex = 0;

                for (int z = 0; z < Chunk.Depth; z++)
                {
                    for (int y = 0; y < Chunk.Height; y++)
                    {
                        for (int x = 0; x < Chunk.Width; x++)
                        {
                            blocks[x + Chunk.Width * y + Chunk.Width * Chunk.Height * z] = ItemCache.GetIndex(dc.Palette[dc.Data[chunkindex++]]);
                        }
                    }
                }

                return blocks;
            }
        }
    }
}
