using System;
using WEngine;

namespace Winecrash
{
    [Serializable] public struct ManufacturePattern
    {
        private ItemAmount[] _items;
        public ItemAmount[] Items
        {
            get => _items;
            set => _items = value;
        }

        private Vector2I _size;
        public Vector2I Size
        {
            get => _size;
            set => _size = value;
        }
    }
}