using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

namespace Winecrash.Client
{
    internal static class ItemCache
    {
        private static readonly Dictionary<string, Item> _ItemsIdentifiers = new Dictionary<string, Item>();
        private static readonly Dictionary<Item, ushort> _ItemsListIndexes = new Dictionary<Item, ushort>();
        private static readonly Dictionary<string, ushort> _ItemIdentifiersIndexes = new Dictionary<string, ushort>();
        private static readonly Dictionary<ushort, string> _ItemIndexesIdentifiers = new Dictionary<ushort, string>();
        private static readonly List<Item> _ItemsList = new List<Item>();
        public static ushort TotalItems
        {
            get
            {
                return (ushort)_ItemsList.Count;
            }
        }

        public const ushort TextureSize = 16;

        public static T Get<T>(string identifier) where T : Item
        {
            return _ItemsIdentifiers[identifier] as T;
        }
        public static T Get<T>(int index) where T : Item
        {
            return _ItemsList[index] as T;
        }
        public static ushort GetIndex(Item item)
        {
            return _ItemsListIndexes[item];
        }
        public static ushort GetIndex(string identifier)
        {
            return _ItemIdentifiersIndexes[identifier];
        }
        public static string GetIdentifier(ushort index)
        {
            return _ItemIndexesIdentifiers[index];
        }


        public static bool TryGet<T>(string identifier, out T item) where T : Item
        {
            Item dbItem = _ItemsIdentifiers[identifier];

            if(dbItem is T actualItem)
            {
                item = actualItem;
                return true;
            }

            else
            {
                item = null;
                return false;
            }
        }
        public static bool TryGet<T>(int index, out T item) where T : Item
        {
            Item dbItem = _ItemsList[index];

            if (dbItem is T actualItem)
            {
                item = actualItem;
                return true;
            }

            else
            {
                item = null;
                return false;
            }
        }
        internal static void AddItems(IEnumerable<Item> items)
        {
            ushort baseIdx = (ushort)_ItemsList.Count;
            foreach (Item item in items)
            {
                _ItemsIdentifiers.Add(item.Identifier, item);
                _ItemsListIndexes.Add(item, baseIdx);
                _ItemIdentifiersIndexes.Add(item.Identifier, baseIdx);
                _ItemIndexesIdentifiers.Add(baseIdx, item.Identifier);

                baseIdx++;
            }

            _ItemsList.AddRange(items);
        }

        private static uint NextPowerOfTwo(uint x)
        {
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            //x |= x >> 16;
            x++;

            return x;
        }

        public static Texture BuildChunkTexture(out int xSize, out int ySize)
        {
            int texsize = (int)TextureSize;
            int totalHeight = texsize * (int)TotalItems;
            int totalWidth = texsize * 6; //6 faces

            xSize = totalWidth;
            ySize = totalHeight;

            Texture tex = new Texture(totalWidth, totalHeight);

            for (int y = 0; y < (int)TotalItems; y++)
            {
                if(ItemCache.TryGet<Cube>(y, out Cube c))
                {
                    //East
                    tex.SetPixels(texsize * 0, y * texsize, texsize, texsize, c.EastTexture.GetPixels(0, 0, texsize, texsize));
                    //West
                    tex.SetPixels(texsize * 1, y * texsize, texsize, texsize, c.WestTexture.GetPixels(0, 0, texsize, texsize));

                    //Up
                    tex.SetPixels(texsize * 2, y * texsize, texsize, texsize, c.UpTexture.GetPixels(0, 0, texsize, texsize));
                    //Down
                    tex.SetPixels(texsize * 3, y * texsize, texsize, texsize, c.DownTexture.GetPixels(0, 0, texsize, texsize));


                    //North
                    tex.SetPixels(texsize * 4, y * texsize, texsize, texsize, c.NorthTexture.GetPixels(0,0, texsize, texsize));
                    //South
                    tex.SetPixels(texsize * 5, y * texsize, texsize, texsize, c.SouthTexture.GetPixels(0, 0, texsize, texsize));
                }
            }


            tex.Apply();

            //Texture tex1 = ItemCache.Get<Cube>("winecrash:grass").NorthTexture;
            return tex;

        }
    }
}
