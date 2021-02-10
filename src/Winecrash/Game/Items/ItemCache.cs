using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;

namespace Winecrash
{
    public static class ItemCache
    {
        private static readonly Dictionary<string, Item> _ItemsIdentifiers = new Dictionary<string, Item>();
        private static readonly Dictionary<Item, ushort> _ItemsListIndexes = new Dictionary<Item, ushort>();
        private static readonly Dictionary<string, ushort> _ItemIdentifiersIndexes = new Dictionary<string, ushort>();
        private static readonly Dictionary<ushort, string> _ItemIndexesIdentifiers = new Dictionary<ushort, string>();
        private static readonly List<Item> _ItemsList = new List<Item>();

        private static readonly object _itemAddLocker = new object();
        
        public static Texture Atlas { get; set; }
        public static Material AtlasMaterial { get; private set; }
        
        
        public static ushort TotalItems
        {
            get
            {
                return (ushort)_ItemsList.Count;
            }
        }

        public const ushort TextureSize = 16;

        public static Item Get(string identifier) => _ItemsIdentifiers[identifier];
        public static T Get<T>(string identifier) where T : Item => Get(identifier) as T;
        
        public static Item Get(int index) => _ItemsList[index];
        public static T Get<T>(int index) where T : Item => Get(index) as T;
        public static ushort GetIndex(Item item) => _ItemsListIndexes[item];
        public static ushort GetIndex(string identifier) => _ItemIdentifiersIndexes[identifier];
        
        public static string GetIdentifier(ushort index) =>_ItemIndexesIdentifiers[index];
        
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

        internal static void AddItem(Item item)
        {
            lock (_itemAddLocker)
            {
                ushort i = (ushort)_ItemsList.Count;
                _ItemsIdentifiers.Add(item.Identifier, item);
                _ItemsListIndexes.Add(item, i);
                _ItemIdentifiersIndexes.Add(item.Identifier, i);
                _ItemIndexesIdentifiers.Add(i, item.Identifier);
                
                _ItemsList.Add(item);
            }
        }
        
        public static Texture BuildChunkTexture(out int xSize, out int ySize)
        {
            int texsize = (int)TextureSize;
            int totalHeight = texsize * (int)TotalItems;
            int totalWidth = texsize * 6; //6 faces

            xSize = totalWidth;
            ySize = totalHeight;

            Texture tex = Atlas = new Texture(totalWidth, totalHeight)
            {
                Name = "ItemAtlas"
            };

            //for (int y = 0; y < (int)TotalItems; y++)
            Parallel.For(0, (int) TotalItems, y =>
            {
                Item item = Get<Item>(y);
                
                
                if (item is Cube c)
                {
                    //East
                    tex.SetPixels(texsize * 0, y * texsize, texsize, texsize,
                        c.EastTexture.GetPixels(0, 0, texsize, texsize));
                    //West
                    tex.SetPixels(texsize * 1, y * texsize, texsize, texsize,
                        c.WestTexture.GetPixels(0, 0, texsize, texsize));

                    //Up
                    tex.SetPixels(texsize * 2, y * texsize, texsize, texsize,
                        c.UpTexture.GetPixels(0, 0, texsize, texsize));
                    //Down
                    tex.SetPixels(texsize * 3, y * texsize, texsize, texsize,
                        c.DownTexture.GetPixels(0, 0, texsize, texsize));


                    //North
                    tex.SetPixels(texsize * 4, y * texsize, texsize, texsize,
                        c.NorthTexture.GetPixels(0, 0, texsize, texsize));
                    //South
                    tex.SetPixels(texsize * 5, y * texsize, texsize, texsize,
                        c.SouthTexture.GetPixels(0, 0, texsize, texsize));
                }
                else if(item.Texture != null)
                {
                    tex.SetPixels(texsize * 0, y * texsize, texsize, texsize,
                        item.Texture.GetPixels(0,0,texsize,texsize));
                }
            });


            tex.Apply();
            
            Material m = AtlasMaterial = new Material(Shader.Find("Item"));
            m.SetData("albedo", tex);
            m.SetData("color", new Color256(1, 1, 1, 1));
            m.SetData("minLight", 0.8F);
            m.SetData("maxLight", 1.0F);
            
            return tex;
        }
    }
}