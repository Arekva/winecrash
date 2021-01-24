using System;
using System.ComponentModel;
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
            
            if (previousContainer != null)
            {
                OnContainerClose?.Invoke(previousContainer);
            }
            else
            {
                OnContainerToggle?.Invoke(newContainer);
            }
            
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

        /// <summary>
        /// 0 - 8 => Hotbar
        /// <br>9 - 17 => First inv. line (bottom)</br>
        /// <br>18 - 26 => Second inv. line (middle)</br>
        /// <br>27 - 35 => Third inv. line (top)</br>
        /// <br>36 - 39 => Armor inv. (top to bottom: 36 head, 37 torso, 38 legs, 39 feet)</br>
        /// <br>40 - 43 => Crafting inv. (numbers disposed in latin reading way)</br>
        /// <br>44 => Crafting result.</br>
        /// </summary>
        public ContainerItem[] Items { get; set; } = new ContainerItem[45];
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

        
        public void SetContainerItem(ContainerItem item, int index)
        {
            if(item == null || item.Amount == 0) item = new ContainerItem();
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
                    break;
                case PlayerContainerTypes.CraftOutput:
                    OnCraftOutputUpdate?.Invoke(item, index);
                    break;
            }
            
            OnItemUpdate?.Invoke(item, index);

            if (!item.Item.Equals(previousItem.Item))
            {
                OnItemRemove?.Invoke(previousItem, index);
                OnItemAdd?.Invoke(item, index);
                previousItem.Delete();
            }
            
            //previousItem.Delete();
        }

        public PlayerContainerTypes GetContainerType(uint index)
        {
            if (index < 9) return PlayerContainerTypes.Hotbar;
            if (index < 36) return PlayerContainerTypes.Bag;
            if (index < 40) return PlayerContainerTypes.Armor;
            if (index < 44) return PlayerContainerTypes.Craft;
            if (index < 45) return PlayerContainerTypes.CraftOutput;

            throw new ArgumentOutOfRangeException(nameof(index), "Container index must be inferior to 45.");
        }
        
        public uint GetContainerIndex(PlayerContainerTypes type, uint shift)
        {
            switch (type)
            {
                case PlayerContainerTypes.Hotbar:
                    if (shift < 9) return shift;
                    throw new ArgumentOutOfRangeException(nameof(shift), "The hotbar being 9 items long, the shift must be inferior to 9.");
                case PlayerContainerTypes.CraftOutput:
                    return 44;
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