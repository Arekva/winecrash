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
        public static short[,,] GetChunk(int x, int y, out bool generated)
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
        private static short[,,] Generate(int chunkx, int chunky)
        {
            short[,,] blocks = new short[Chunk.Width, Chunk.Height, Chunk.Depth];

            LibNoise.Primitive.SimplexPerlin perlin = new LibNoise.Primitive.SimplexPerlin("lol".GetHashCode(), NoiseQuality.Standard);
            
            for (int z = 0; z < Chunk.Depth; z++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        string id = "winecrash:air";

                        const float scale = 0.05F;
                        const float shiftX = 10000;
                        const float shiftZ = 10000;

                        int height = (int)(perlin.GetValue((chunkx * Chunk.Width + shiftX + x) * scale, (chunky * Chunk.Depth + shiftZ + z) * scale) * 10) + 58;

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
                        blocks[x, y, z] = ItemCache.GetCacheIndex(id);//new Block(id);
                    }
                }
            }

            return blocks;
        }
        private static short[,,] LoadFromSave(string path)
        {
            //string json = File.ReadAllText(path);

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = File.OpenText(path))
            using (JsonTextReader jtr = new JsonTextReader(sr))
            {
                DictionnaryChunk dc = (DictionnaryChunk)serializer.Deserialize(jtr, typeof(DictionnaryChunk));

                short[,,] blocks = new short[Chunk.Width, Chunk.Height, Chunk.Depth];
                int chunkindex = 0;

                for (int z = 0; z < Chunk.Depth; z++)
                {
                    for (int y = 0; y < Chunk.Height; y++)
                    {
                        for (int x = 0; x < Chunk.Width; x++)
                        {
                            blocks[x, y, z] = /*new Block(*/ItemCache.GetCacheIndex(dc.Dictionnary[dc.References[chunkindex]])/*)*/;
                            chunkindex++;
                        }
                    }
                }

                return blocks;
            }
        }
    }
}
