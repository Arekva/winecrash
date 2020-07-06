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
        static LibNoise.Primitive.ImprovedPerlin caves = new LibNoise.Primitive.SimplexPerlin("lol".GetHashCode(), NoiseQuality.Standard);

        public static ushort[] Generate(int chunkx, int chunky, bool save = false, bool erase = false)
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

                        const float caveScale = 0.1F;


                        const float thresold = 0.4F;

                        int height = (int)(perlin.GetValue((chunkx * Chunk.Width + shiftX + x) * scale, (chunky * Chunk.Depth + shiftZ + z) * scale) * 5) + 60;

                        bool isCave = (((caves.GetValue((chunkx * Chunk.Width + shiftX + (float)x) * caveScale, y * caveScale, (chunky * Chunk.Depth + shiftZ + (float)z) * caveScale)) + 1) /2.0F) < thresold;

                        
                        if (y == height)
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

                        if(isCave)
                        {
                            id = "winecrash:air";
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

            if(save)
            {
                string fileName = "save/" + $"c{chunkx}_{chunky}.json";

                if(erase)
                    File.WriteAllText(fileName, ToJSON(blocks));

                else if(!File.Exists(fileName))
                {
                    File.WriteAllText(fileName, ToJSON(blocks));
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

        private static string ToJSON(ushort[] blocks)
        {
            Dictionary<string, int> distinctIDs = new Dictionary<string, int>(64);

            int[] blocksRef = new int[Chunk.TotalBlocks];

            int chunkIndex = 0;
            int paletteIndex = 0;

            for (int z = 0; z < Chunk.Depth; z++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        string id = ItemCache.GetIdentifier(blocks[x + Chunk.Width * y + Chunk.Width * Chunk.Height * z]);

                        if (!distinctIDs.ContainsKey(id))
                        {
                            distinctIDs.Add(id, paletteIndex++);
                        }

                        blocksRef[chunkIndex++] = distinctIDs[id];
                    }
                }
            }

            return JsonConvert.SerializeObject(new JSONChunk()
            {
                Palette = distinctIDs.Keys.ToArray(),
                Data = blocksRef
            }, Formatting.None);
        }
    }
}
