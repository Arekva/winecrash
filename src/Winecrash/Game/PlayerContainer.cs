using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using WEngine;

namespace Winecrash
{
    public delegate void ContainerDelegate(IContainer container);
    public partial class Player
    {
        public event ItemChangeDelegate OnItemAdd;
        public event ItemChangeDelegate OnItemRemove;
        public event ItemChangeDelegate OnItemUpdate;
        public event ItemChangeDelegate OnHotbarUpdate;
        public event ItemChangeDelegate OnStorageUpdate;
        public event ItemChangeDelegate OnBagUpdate;
        public event ItemChangeDelegate OnArmorUpdate;
        public event ItemChangeDelegate OnCraftUpdate;
        public event ItemChangeDelegate OnCraftOutputUpdate;
        public event ItemChangeDelegate OnGrabUpdate;
        public event ItemChangeDelegate OnHotbarSelectedChange;
        public event ContainerDelegate OnContainerOpen;
        public event ContainerDelegate OnContainerClose;
        public event ContainerDelegate OnContainerToggle;

        public bool ContainerOpened => OpenedContainer != null;
        public IContainer OpenedContainer { get; private set; } = null;

        private int _HotbarSelectedIndex = 0;
        public int HotbarSelectedIndex
        {
            get => _HotbarSelectedIndex;
            set
            {
                if (!ContainerOpened)
                {
                    while (value < 0)
                    {
                        value += 9;
                    }

                    int newValue = value % 9;
                    if (newValue != _HotbarSelectedIndex)
                    {
                        _HotbarSelectedIndex = newValue;
                        OnHotbarSelectedChange?.Invoke(Hotbar[newValue], newValue);
                    }
                }
            }
        }

        public void OpenContainer(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container), "Cannot open null container !");

            IContainer previousContainer = OpenedContainer;
            IContainer newContainer = OpenedContainer = container;

            if (previousContainer != null) OnContainerClose?.Invoke(previousContainer);
            else OnContainerToggle?.Invoke(newContainer);

            OnContainerOpen?.Invoke(newContainer);
        }

        public void CloseContainer()
        {
            if (OpenedContainer != null)
            {
                OnContainerToggle?.Invoke(OpenedContainer);
                OnContainerClose?.Invoke(OpenedContainer);
            }

            OpenedContainer = null;
        }

        public ContainerItem[] Hotbar
        {
            get
            {
                ContainerItem[] items = new ContainerItem[9];
                Array.Copy(Items, items, 9);
                return items;
            }
        }

        public ContainerItem[] Craft
        {
            get
            {
                ContainerItem[] items = new ContainerItem[4];
                Array.Copy(Items, GetContainerIndex(PlayerContainerTypes.Craft), items, 0, 4);
                return items;
            }
        }

        public ContainerItem Grabbed => Items[GetContainerIndex(PlayerContainerTypes.Grab)];

        /// <summary>
        /// 0 - 8 => Hotbar
        /// <br>9 - 17 => First inv. line (bottom)</br>
        /// <br>18 - 26 => Second inv. line (middle)</br>
        /// <br>27 - 35 => Third inv. line (top)</br>
        /// <br>36 - 39 => Armor inv. (top to bottom: 36 head, 37 torso, 38 legs, 39 feet)</br>
        /// <br>40 - 43 => Crafting inv. (numbers disposed in latin reading way)</br>
        /// <br>44 => Crafting result.</br>
        /// </summary>
        public ContainerItem[] Items { get; set; } = new ContainerItem[46];
        public void AddItemFast(ContainerItem item)
        {
            if (item == null) return;
            
            int n = 0;
            bool placed = false;
            while (!placed && n < 36)
            {
                ContainerItem current = Items[n];

                if (current == null || current.Item.Identifier == "winecrash:air")
                {
                    SetContainerItem(item, n);
                    placed = true;
                }

                else if (current.Item.Identifier == item.Item.Identifier)
                {
                    //if item is already at max, ignore and skip.
                    if (current.Amount >= current.Item.Stack)
                    {
                        n++;
                    }
                    else
                    {
                        int remainingSpace = current.Item.Stack - current.Amount;

                        int toAdd = WMath.Clamp(item.Amount, 1, remainingSpace);

                        current.Amount += (byte) toAdd;
                        item.Amount -= (byte) toAdd;

                        SetContainerItem(current, n);

                        if (item.Amount == 0)
                        {
                            placed = true;
                        }

                        n++;
                    }
                }

                else // if not the same item, don't add to slot and skip to next.
                {
                    n++;
                }
            }
        }

        public KeyValuePair<ManufactureTable, KeyValuePair<ManufacturePattern, Vector2I>> CraftResult;

        public void MainGrab(int index)
        {
            ContainerItem current = Grabbed;
            ContainerItem item = Items[index];

            // if index's item is not empty
            if (item && item.Item != null && item.Item.Identifier != "winecrash:air" && item.Amount > 0)
            {
                // if no item already grabbed, take the index one
                if (index == GetContainerIndex(PlayerContainerTypes.CraftOutput)) // craft
                {
                    if (current != null && current.Valid)
                    {
                        ContainerItem result = (ContainerItem)CraftResult.Key.Results[0];
                        if (result.Item.Identifier == current.Item.Identifier)
                        {
                            byte remaining = (byte) (current.Item.Stack - current.Amount);
                            if (remaining >= result.Amount) // if still has space for craft output
                            {
                                ContainerItem output = (ContainerItem) ValidateCraft();
                                current.Amount += output.Amount;
                                SetContainerItem(current.Duplicate(), GetContainerIndex(PlayerContainerTypes.Grab));
                                CheckCraft();
                            }
                            else // otherwise don't grab
                            {
                                SetContainerItem(current.Duplicate(), GetContainerIndex(PlayerContainerTypes.Grab));
                                SetContainerItem(item.Duplicate(), index);
                            }
                        }
                        else // otherwise don't grab
                        {
                            SetContainerItem(current.Duplicate(), GetContainerIndex(PlayerContainerTypes.Grab));
                            SetContainerItem(item.Duplicate(), index);
                        }
                    }
                    else
                    {
                        SetContainerItem((ContainerItem) ValidateCraft(), GetContainerIndex(PlayerContainerTypes.Grab));
                        CheckCraft();
                    }
                }
                else if (!current || !current.Valid) // non craft
                {
                    SetContainerItem(item.Duplicate(), GetContainerIndex(PlayerContainerTypes.Grab));
                    SetContainerItem(null, index);
                }
                // if grabbed and index
                else
                {
                    // same type, append
                    if (current.Item.Identifier == item.Item.Identifier)
                    {
                        byte remaining = (byte) (item.Item.Stack - item.Amount);
                        byte currentQty = current.Amount;

                        byte toPlace = Math.Min(currentQty, remaining);

                        item.Amount += toPlace;
                        current.Amount -= toPlace;

                        SetContainerItem(item.Duplicate(), index);
                        SetContainerItem(current.Duplicate(), GetContainerIndex(PlayerContainerTypes.Grab));
                    }
                    else // exchange
                    {
                        ContainerItem ncurrent = current.Duplicate();
                        ContainerItem nitem = item.Duplicate();
                        SetContainerItem(ncurrent, index);
                        SetContainerItem(nitem, GetContainerIndex(PlayerContainerTypes.Grab));
                    }
                }
            }
            // simply place grab item in index
            else if (current && current.Item != null && current.Item.Identifier != "winecrash:air")
            {
                SetContainerItem(current.Duplicate(), index);
                SetContainerItem(null, GetContainerIndex(PlayerContainerTypes.Grab));
            }

            item?.Delete();
            current?.Delete();
        }

        public void AltGrab(int index)
        {
            ContainerItem current = Grabbed;
            ContainerItem item = Items[index];

            // if there is something grab
            if (current && current.Valid)
            {
                if (!item || !item.Valid || current.Item.Identifier == item.Item.Identifier) // if items are same type
                { // place one
                    byte remaining = !item || !item.Valid ? (byte)1 : (byte)(item.Item.Stack - item.Amount);
                    if (remaining != 0)
                    {
                        item = item && item.Valid ? new ContainerItem(item.Item, (byte)(item.Amount + 1)) : new ContainerItem(current.Item);
                        current.Amount -= 1;
                        
                        SetContainerItem(item.Duplicate(), index);
                        SetContainerItem(current.Duplicate(), (int)GetContainerIndex(PlayerContainerTypes.Grab));
                    }
                }
                else // exchange
                {
                    ContainerItem ncurrent = current.Duplicate();
                    ContainerItem nitem = item.Duplicate();
                    SetContainerItem(ncurrent, index);
                    SetContainerItem(nitem, (int)GetContainerIndex(PlayerContainerTypes.Grab));
                }
            } else { // nothing in hand
                if (item && item.Valid)
                { // if there is something to grab
                    byte half = (byte)Math.Max(1.0, Math.Ceiling(item.Amount/2.0));
                    
                    SetContainerItem(new ContainerItem(item.Item, half), (int)GetContainerIndex(PlayerContainerTypes.Grab));
                    item.Amount -= half;
                    
                    SetContainerItem(item.Duplicate(), index);
                }
            }

            item?.Delete();
            current?.Delete();
        }

        public void SetContainerItem(ContainerItem item, int index)
        {
            if(item == null || item.Amount == 0) item = new ContainerItem("winecrash:air");
            ContainerItem previousItem = Items[index];
            
            if(previousItem == null) previousItem = new ContainerItem();

            Items[index] = item;
            
            switch (GetContainerType((uint)index))
            {
                case PlayerContainerTypes.Hotbar:
                    OnHotbarUpdate?.Invoke(item, index);
                    OnStorageUpdate?.Invoke(item, index);
                    break;
                case PlayerContainerTypes.Bag:
                    OnBagUpdate?.Invoke(item, index);
                    OnStorageUpdate?.Invoke(item, index);
                    break;
                case PlayerContainerTypes.Storage:
                    OnHotbarUpdate?.Invoke(item, index);
                    OnBagUpdate?.Invoke(item, index);
                    OnStorageUpdate?.Invoke(item, index);
                    break;
                case PlayerContainerTypes.Armor:
                    OnArmorUpdate?.Invoke(item, index);
                    break;
                case PlayerContainerTypes.Craft:
                    OnCraftUpdate?.Invoke(item, index);
                    CheckCraft();
                    break;
                case PlayerContainerTypes.CraftOutput:
                    OnCraftOutputUpdate?.Invoke(item, index);
                    break;
                case PlayerContainerTypes.Grab:
                    OnGrabUpdate?.Invoke(item, index);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            OnItemUpdate?.Invoke(item, index);

            if (!item.Item.Equals(previousItem.Item))
            {
                OnItemRemove?.Invoke(previousItem, index);
                OnItemAdd?.Invoke(item, index);
                previousItem.Delete();
            }
        }

        public void CheckCraft()
        {
            Vector2I playerCraftSize = Vector2I.One * 2;

            ContainerItem[] containerItems = Player.LocalPlayer.Craft;
            ItemAmount[] itemAmounts = new ItemAmount[containerItems.Length];
            for (int i = 0; i < containerItems.Length; i++) itemAmounts[i] = (ItemAmount)containerItems[i];

            bool found = false;
            foreach (ManufactureTable table in ManufactureTable.Cache)
            {
                if (table.Validate(itemAmounts, playerCraftSize, out KeyValuePair<ManufacturePattern, Vector2I> result))
                {
                    CraftResult = new KeyValuePair<ManufactureTable, KeyValuePair<ManufacturePattern, Vector2I>>(table, result);
                    // set output
                    Player.LocalPlayer.SetContainerItem((ContainerItem)table.Results[0], Player.GetContainerIndex(PlayerContainerTypes.CraftOutput));
                    found = true;
                    break;
                }
            }

            if (!found) Player.LocalPlayer.SetContainerItem(null, Player.GetContainerIndex(PlayerContainerTypes.CraftOutput));
        }

        public ItemAmount ValidateCraft()
        {
            if (CraftResult.Value.Key.Items == null) return new ItemAmount();
            
            // edit input
            ManufactureTable table = CraftResult.Key;
            ManufacturePattern pattern = CraftResult.Value.Key;
            Vector2I shift = CraftResult.Value.Value;
            ItemAmount[] pitems = pattern.Items;
            
            ContainerItem[] containerItems = Player.LocalPlayer.Craft;
            ItemAmount[] itemAmounts = new ItemAmount[containerItems.Length];
            for (int i = 0; i < containerItems.Length; i++) itemAmounts[i] = (ItemAmount)containerItems[i];

            for (int i = 0; i < pitems.Length; i++)
            {
                ItemAmount ia = pitems[i];
                
                if (ia.Amount == 0 || ia.Identifier == "winecrash:air") continue;
                
                // pattern space
                WMath.FlatTo2D(i, pattern.Size.X, out int x, out int y);
                
                // craft space
                Vector2I cpos = new Vector2I(x, y) + shift;

                int k = WMath.Flatten2D(cpos.X, cpos.Y, 2);

                itemAmounts[k].Amount -= ia.Amount;
                
                Player.LocalPlayer.SetContainerItem((ContainerItem)itemAmounts[k], Player.GetContainerIndex(PlayerContainerTypes.Craft, k));
            }

            return table.Results[0];
        }

        public static PlayerContainerTypes GetContainerType(uint index)
        {
            if (index < 9) return PlayerContainerTypes.Hotbar;
            if (index < 36) return PlayerContainerTypes.Bag;
            if (index < 40) return PlayerContainerTypes.Armor;
            if (index < 44) return PlayerContainerTypes.Craft;
            if (index < 45) return PlayerContainerTypes.CraftOutput;
            if (index < 46) return PlayerContainerTypes.Grab;

            throw new ArgumentOutOfRangeException(nameof(index), "Container index must be inferior to 45.");
        }
        
        public static int GetContainerIndex(PlayerContainerTypes type, int shift = 0)
        {
            switch (type)
            {
                case PlayerContainerTypes.Hotbar:
                    if (shift < 9) return shift;
                    throw new ArgumentOutOfRangeException(nameof(shift), "The hotbar being 9 items long, the shift must be inferior to 9.");
                case PlayerContainerTypes.CraftOutput:
                    return 44;
                case PlayerContainerTypes.Grab:
                    return 45;
                case PlayerContainerTypes.Craft:
                    if (shift < 4) return 40 + shift;
                    throw new ArgumentOutOfRangeException(nameof(shift), "The crafting slots being 4 (2x2) items long, the shift must be inferior to 4.");
                case PlayerContainerTypes.Armor:
                    if (shift < 4) return 36 + shift;
                    throw new ArgumentOutOfRangeException(nameof(shift), "The armor slots being 4 items long, the shift must be inferior to 4.");
                case PlayerContainerTypes.Bag:
                    if (shift < 27) return 9 + shift;
                    throw new ArgumentOutOfRangeException(nameof(shift), "The bag slots being 27 (3x9) items long, the shift must be inferior to 27.");
                case PlayerContainerTypes.Storage:
                    if (shift < 36) return shift;
                    throw new ArgumentOutOfRangeException(nameof(shift), "The storage slots being 27 (3x9 + 1x9) items long, the shift must be inferior to 36.");
                default:
                    throw new ArgumentException($"Invalid type \"{type.ToString()}\"", nameof(type));
            }
        }
    }
}