using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WEngine;

namespace Winecrash
{
    [Serializable] public struct ManufactureTable
    {
        public static List<ManufactureTable> Tables { get; internal set; } = new List<ManufactureTable>();
        
        private ItemAmount[] _results;
        public ItemAmount[] Results
        {
            get => _results;
            set => _results = value;
        }

        private ManufacturePattern[] _patterns;
        public ManufacturePattern[] Patterns
        {
            get => _patterns;
            set => _patterns = value;
        }

        private Vector2I _size;
        public Vector2I Size
        {
            get => _size;
            set => _size = value;
        }

        private Type[] _manufactures;
        public Type[] Manufactures
        {
            get => _manufactures;
            set => _manufactures = value;
        }
        
        public bool Validate(IEnumerable<ItemAmount> input, Vector2I workspaceSize, out KeyValuePair<ManufacturePattern, Vector2I> result)
        {
            // sanitize params
            if (input == null) throw new ArgumentNullException(nameof(input), "Please provide input items.");
            if (workspaceSize.X < 1 || workspaceSize.Y < 1) throw new ArgumentException("Please provide a valid size (at least 1x1).", nameof(workspaceSize));
            ItemAmount[] inputItems = input as ItemAmount[] ?? input.ToArray();
            if (inputItems.Length != workspaceSize.X+workspaceSize.Y) throw new ArgumentException("Please provide the same input quantity as provided size.", nameof(input));

            for (int i = 0; i < inputItems.Length; i++)
            {
                ItemAmount it = inputItems[i];
                if (it.Identifier == null || it.Identifier == "winecrash:air")
                {
                    it.Identifier = null;
                    it.Amount = 0;
                }
                inputItems[i] = it;
            }
            
            result = new KeyValuePair<ManufacturePattern, Vector2I>();
            
            for (int i = 0; i < Patterns.Length; i++)
            {   // for each pattern
                ManufacturePattern pattern = Patterns[i];
                ItemAmount[] items = pattern.Items;

                // if same sequence, ok
                if (items.SequenceEqual(inputItems)) return true;
                
                // otherwise test all possibilities
                Vector2I size = pattern.Size;
                // if the pattern is too big to fit into the workspace, skip
                if (size.X > workspaceSize.X || size.Y > workspaceSize.Y) continue;

                // the algo. will virtual move the pattern all around the
                // workspace and check if it does match the inputs
                Vector2I maxShift = workspaceSize - size;
                
                // for each of the virtual move
                for (int sy = 0; sy < maxShift.Y + 1; sy++)
                {
                    for (int sx = 0; sx < maxShift.X + 1; sx++)
                    {
                        ItemAmount[] virtualPattern = new ItemAmount[workspaceSize.X * workspaceSize.Y];
                        Vector2I shift = new Vector2I(sx, sy);
                        for (int j = 0; j < items.Length; j++)
                        {
                            // retrieve (X;Y) coordinates within pattern space
                            WMath.FlatTo2D(j, size.X, out int patternX, out int patternY);
                            
                            // move into virtual space
                            Vector2I coords = new Vector2I(patternX, patternY) + shift;
                            
                            // retrieve virtual index for said coordinates
                            int virtualIndex = WMath.Flatten2D(coords.X, coords.Y, workspaceSize.X);

                            virtualPattern[virtualIndex] = items[j];
                        }

                        // check if both virtual and input sequences are equal.
                        // ItemAmounts are set to be equal if both have the
                        // winecrash:atmosphere identifier or both amounts are 0
                        if (virtualPattern.SequenceEqual(inputItems))
                        {
                            // found !
                            result = new KeyValuePair<ManufacturePattern, Vector2I>(pattern, shift);
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
    }
}