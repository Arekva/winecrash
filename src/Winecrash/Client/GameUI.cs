using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WEngine;
using WEngine.GUI;
using Winecrash.Entities;
using Winecrash.GUI;
using Keys = WEngine.Keys;
using Label = WEngine.GUI.Label;

namespace Winecrash.Client
{
    public class GameUI : Module
    {
        public WObject Crosshair;
        public Image HotbarImage;
        public Image HotbarSelectorImage;
        public WObject Hotbar;
        public WObject HotbarSelector;
        public WObject DebugPanel;
        public Label DebugPosition;
        public WObject Inventory;
        public WObject GlobalInventoryBackground;
        public Label ItemNameLabel;

        public static double UIScale { get; } = -0.05D;
        
        public static GameUI Instance { get; private set; }

        private static readonly Vector2D[][] _ItemHotbarAnchors = new Vector2D[9][];
        

        private static WObject[] HotbarItemsRenders { get; set; } = new WObject[9];
        private static Label[] HotbarItemsAmount { get; set; } = new Label[9];
        
        private static readonly Vector2D[][] _ItemInventoryAnchors = new Vector2D[46][];
        private static WObject[] InventoryItemsRenders { get; set; } = new WObject[46];
        private static Label[] InventoryItemsAmount { get; set; } = new Label[46];

        private static PointBoxCollisionProvider _collisionProvider = new PointBoxCollisionProvider();

        private static UIBoxCollider[] _colliders = new UIBoxCollider[46];
        
        public WObject HoveredTooltipWObject { get; set; }
        public ToolTip HoveredTooltip { get; set; }

        public static Vector2D ItemSlotScale
        {
            get
            {
                Vector2D[] sizes = ItemSlotBaseAnchors;
                Vector2D slotMins = sizes[0];
                Vector2D slotMaxs = sizes[1];

                Vector2D deltas = slotMaxs - slotMins;
                Vector2D screenResolution = Graphics.Window.SurfaceResolution;

                Vector2D actualSizes = deltas * screenResolution;
                return actualSizes / 4.0;
            }
        }

        public static Vector2D[] ItemSlotBaseAnchors => new Vector2D[2]
        {
            new Vector2D((3.0 / 182.0), 0.0),
            new Vector2D((19.0 / 182.0), 3.0 / 22.0)
        };

        public static Vector2D[] ItemSlotBase => new Vector2D[2]
        {
            new Vector2D(0, 0.0),
            new Vector2D((19.0 / 182.0) - (3.0 / 182.0), 3.0 / 22.0)
        };
        


        //private static Material MenuBackgroundMaterial = new Material(Shader.Find("Unlit")) { RenderOrder = 10 };
        //private static Material MenuInventoryMaterial = new Material(Shader.Find("Unlit")) { RenderOrder = 100 };
        
        static GameUI()
        {
            CreateHotbarAnchors();
            CreateInventoryAnchors();
        }

        private static void CreateHotbarAnchors()
        {
            const double baseMinX = 3.0D;
            const double minY = 19 / 22.0D;
            
            const double baseMaxX = 19.0D;
            const double maxY = 3 / 22.0D;
            
            const double divisor = 182.0D;
            const double increment = 20.0D;

            for (int i = 0; i < _ItemHotbarAnchors.Length; i++)
            {
                _ItemHotbarAnchors[i] = new Vector2D[2]
                {
                    new Vector2D((baseMinX + increment*i) / divisor, minY), // min
                    new Vector2D((baseMaxX + increment*i) / divisor, maxY)  // max
                };
            }
        }

        private static void CreateInventoryAnchors()
        {
            Vector2D shift;

            // hotbar 
            for (int i = 0; i < 9; i++)
            {
                const double baseMinX = 3.0D;
                const double minY = 0.0D;
            
                const double baseMaxX = 19.0D;
                const double maxY = 3 / 22.0D;
            
                const double divisor = 182.0D;
                const double incrementX = 20.0D;
                const double incrementY = 0.12325D;

                shift = new Vector2D(0,-0.0025);
                
                WMath.FlatTo2D(i, 9, out int x, out int y);
                _ItemInventoryAnchors[i] = new Vector2D[2]
                {
                    new Vector2D((baseMinX + incrementX*x) / divisor, minY + incrementY*y) + shift, // min
                    new Vector2D((baseMaxX + incrementX*x) / divisor, maxY + incrementY*y) + shift // max
                };
            }
            // bag
            for (int i = 9; i < 36; i++)
            {
                const double baseMinX = 3.0D;
                const double minY = 0.0D;
            
                const double baseMaxX = 19.0D;
                const double maxY = 3 / 22.0D;
            
                const double divisor = 182.0D;
                const double incrementX = 20.0D;
                const double incrementY = 0.1205D;

                shift = new Vector2D(0.0,0.0025);
                
                WMath.FlatTo2D(i, 9, out int x, out int y);
                _ItemInventoryAnchors[i] = new Vector2D[2]
                {
                    new Vector2D((baseMinX + incrementX*x) / divisor, minY + incrementY*y) + shift, // min
                    new Vector2D((baseMaxX + incrementX*x) / divisor, maxY + incrementY*y) + shift // max
                };
            }
            // armor
            for (int i = 36, y = 0; i < 40; i++, y++)
            {
                shift = new Vector2D(0, 0.865 - y*0.1205);
                _ItemInventoryAnchors[i] = new Vector2D[2]
                {
                    new Vector2D((3.0 / 182.0), 0.0) + shift, // min
                    new Vector2D((19.0 / 182.0), 3.0 / 22.0) + shift // max
                };
            }
            // crafting
            for (int i = 40; i < 44; i++)
            {
                int x = i == 41 || i == 43 ? 1 : 0;
                int y = i == 42 || i == 43 ? 1 : 0;
                
                shift = new Vector2D(0.495 + (20.0*x/182.0), 0.8645 - 0.1255 - y*0.1205);
                
                _ItemInventoryAnchors[i] = new Vector2D[2]
                {
                    new Vector2D((3.0 / 182.0), 0.0) + shift, // min
                    new Vector2D((19.0 / 182.0), 3.0 / 22.0) + shift // max
                };
            }
            
            // crafting result
            shift = new Vector2D(0.825, 0.6725);
            _ItemInventoryAnchors[44] = new Vector2D[2]
            {
                new Vector2D((3.0 / 182.0), 0.0) + shift, // min
                new Vector2D((19.0 / 182.0), 3.0 / 22.0) + shift // max
            };
            
            // grabbed
            Vector2D[] @base = ItemSlotBase;
            shift = new Vector2D(-0.0275, -0.0625);
            _ItemInventoryAnchors[45] = new Vector2D[2]
            {
                new Vector2D(0.5) - @base[0] + shift,
                new Vector2D(0.5) + @base[1] + shift
            };
        }
        
        protected override void Creation()
        {
            if (Instance)
            {
                this.Delete();
                return;
            }

            Instance = this;
            
            Crosshair = new WObject("Crosshair")
            {
                Parent = Canvas.Main.WObject,
                Layer = (ulong)Layers.UI
            };
            Image img = Crosshair.AddModule<Image>();
            img.Picture = new Texture("assets/textures/gui/crosshair.png");
            img.KeepRatio = true;
            img.MinAnchor = new Vector2D(0.49D, 0.49D);
            img.MaxAnchor = new Vector2D(0.51D, 0.51D);
            img.Material.SourceColorBlending = BlendingFactorSrc.OneMinusDstColor;
            
            
            Hotbar = new WObject("Hotbar")
            {
                Parent = Canvas.Main.WObject,
                Layer = (ulong)Layers.UI
            }; 
            HotbarImage = img = Hotbar.AddModule<Image>();
            img.Picture = new Texture("assets/textures/gui/hotbar.png");
            img.KeepRatio = true;
            img.Depth = 2000;
            img.Renderer.Material.RenderOrder = -10;
            
            HotbarSelector = new WObject("Hotbar Selector")
            {
                Parent = Hotbar,
                Layer = (ulong)Layers.UI
            }; 
            HotbarSelectorImage = img = HotbarSelector.AddModule<Image>();
            img.Picture = new Texture("assets/textures/gui/hotbar_selector.png");
            img.KeepRatio = true;
            img.MinAnchor = new Vector2D(-0.0055D, 0.01D);
            img.MaxAnchor = new Vector2D(0.1265, 1.0725D);
            img.Depth = 1999;

            DebugPanel = new WObject("Debug Panel")
            {
                Parent = Canvas.Main.WObject,
                Layer = (ulong)Layers.UI
            };
            img = DebugPanel.AddModule<Image>();
            img.Color = Color256.Transparent;

            WObject inamewobj = new WObject("Item Name Display")
            {
                Parent = Canvas.Main.WObject,
                Layer = (ulong) Layers.UI
            };

            ItemNameLabel = inamewobj.AddModule<LocalizedLabel>();
            ItemNameLabel.AutoSize = true;
            ItemNameLabel.MinAnchor = new Vector2D(0.33, 0.15);
            ItemNameLabel.MaxAnchor = new Vector2D(0.66, 0.20);
            ItemNameLabel.Aligns = TextAligns.Middle;
            ItemNameLabel.Enabled = false;
            ItemNameLabel.Depth = 1000;
            
            WObject positionTextObj = new WObject("Global Position")
            {
                Parent = DebugPanel
            };
            DebugPosition = positionTextObj.AddModule<Label>();
            DebugPosition.MinAnchor = new Vector2D(0.3,0.95);
            DebugPosition.MaxAnchor = new Vector2D(0.7, 1.0);
            DebugPosition.AutoSize = true;
            DebugPosition.Aligns = TextAligns.Up | TextAligns.Middle;
            DebugPosition.Color = Color256.White;
            DebugPosition.Text = "XYZ: ?/?/?";

            CreateInventoryFocus();
            CreatePlayerInventory();

            Player.LocalPlayer.OnHotbarUpdate += UpdateHotbarItem;
            Player.LocalPlayer.OnHotbarSelectedChange += UpdateHotbarSelected;
            Player.LocalPlayer.OnContainerToggle += TogglePlayerInventory;
            Player.LocalPlayer.OnContainerClose += PlayerCloseInventory;
            Player.LocalPlayer.OnContainerOpen += PlayerOpenInventory;
            Player.LocalPlayer.OnItemUpdate += UpdateItem;

            Player.LocalPlayer.OnCraftUpdate += UpdateCraft;


            HoveredTooltipWObject = new WObject("Item Tooltip")
            {
                Parent = Canvas.Main.WObject,
                Layer = (ulong)Layers.UI,
                Enabled = false
            };
            HoveredTooltip = HoveredTooltipWObject.AddModule<ToolTip>();
            HoveredTooltip.MinAnchor = new Vector2D(0);
            HoveredTooltip.MaxAnchor = new Vector2D(0.2, 0.05);

            CreateInventoryRenders();
        }
        private void CreateInventoryFocus()
        {
            GlobalInventoryBackground = new WObject("Inventory Background")
            {
                Parent = Canvas.Main.WObject,
                Layer = (ulong) Layers.UI
            };

            Image img = GlobalInventoryBackground.AddModule<Image>();
            //img.Material = MenuBackgroundMaterial;
            //img.SetupMaterial();
            img.Picture = Texture.Blank;
            img.Color = new Color256(0.2,0.2,0.2,0.75);
            //GlobalInventoryBackground.GetModule<ImageRenderer>().DrawOrder = 1;
            img.Depth = 0;
            GlobalInventoryBackground.Enabled = false;
        }
        private void CreatePlayerInventory()
        {
            Inventory = new WObject("Player Inventory")
            {
                Parent = Canvas.Main.WObject,
                Layer = (ulong) Layers.UI
            };
            
            Image img = Inventory.AddModule<Image>();
            img.KeepRatio = true;
            img.Picture = new Texture("assets/textures/gui/inventory.png", "Player Inventory", true);
            img.MinAnchor = new Vector2D(0.33, 0.0);
            img.MaxAnchor = new Vector2D(0.66, 1.0);
            img.Depth = -1;

            WObject wobj = new WObject("Crafting Label")
            {
                Parent = Inventory,
                Layer = (ulong) Layers.UI
            };

            LocalizedLabel lb = wobj.AddModule<LocalizedLabel>();
            lb.Localization = "#winecrash:ui.inventory.crafting";
            lb.AutoSize = true;
            lb.Aligns = TextAligns.Left;
            lb.MinAnchor = new Vector2D(0.5, 0.90);
            lb.MaxAnchor = new Vector2D(0.95, 0.965);
            lb.Color = new Color256(0.35,0.35,0.35,1.0);
            lb.Shadows = false;

            Inventory.Enabled = false;
        }
        private void PlayerOpenInventory(IContainer container)
        {
            Input.LockMode = CursorLockModes.Free;
            Input.CursorVisible = true;

            if (container is Player player && player == Player.LocalPlayer) // if player inventory
            {
                Inventory.Enabled = true;
            }
        }
        private void PlayerCloseInventory(IContainer container)
        {
            Input.IgnoreNextFocusFrame = true;
            Input.LockMode = CursorLockModes.Lock;
            Input.CursorVisible = false;
            Inventory.Enabled = false;
        }
        private void TogglePlayerInventory(IContainer container)
        {
            GlobalInventoryBackground.Enabled = !GlobalInventoryBackground.Enabled;

            //Player.LocalPlayer.SetContainerItem(new ContainerItem("winecrash:diamond_pickaxe"), 36);
        }
        
        private WObject itemPreview;

        private static double ItemNameCooldown = 0.0D;
        public static double ItemNameTime = 2.0D;
        public static double ItemNameDisappearTime = 1.0D;

        public static int HoveringIndex()
        {
            Vector2I rawMP = Input.MousePosition;
            Vector3D mp = new Vector3D(rawMP.X, rawMP.Y, 0);
            
            for (int i = 0; i < 45; i++)
            {
                UIBoxCollider collider = _colliders[i];
                if(!collider) continue;
                
                Hit hit = _collisionProvider.Collide(mp, collider);

                if (hit.HasHit) return i;
            }

            return -1;
        }

        private void ItemDisplayName(Item item)
        {
            if (item == null || item.Identifier == "winecrash:air") return;

            ItemNameLabel.Enabled = true;
            ItemNameLabel.Text = Game.Language.GetText(item.DisplayName);
            ItemNameLabel.Color = item.DisplayColor;
            ItemNameCooldown = ItemNameDisappearTime + ItemNameTime;
        }
        private void AnimateDisplayName()
        {
            ItemNameCooldown -= Time.Delta;
            
            if(ItemNameCooldown < 0.0)
                ItemNameLabel.Enabled = false;
            else if (ItemNameCooldown < ItemNameTime)
            {
                Color256 col = ItemNameLabel.Color;
                col.A = WMath.Clamp(ItemNameCooldown / ItemNameDisappearTime,0,1);
                ItemNameLabel.Color = col;
            }
        }

        private void CreateInventoryRenders()
        {
            for (int i = 0; i < InventoryItemsRenders.Length; i++)
            {
                WObject itemRenderer = InventoryItemsRenders[i] = CreateItemRenderer(Inventory, out Model mod, out Label amount);
                itemRenderer.Name += " (Inventory)";
                UIBoxCollider collider = _colliders[i] = itemRenderer.AddModule<UIBoxCollider>();
                collider.GUIModule = mod;
                Vector2D scales = ItemSlotScale;
                collider.Extents = new Vector3D(scales, 10000);
                mod.Depth = -1000;
                amount.Depth = -1000;
                InventoryItemsAmount[i] = amount;
                
                mod.MinAnchor = _ItemInventoryAnchors[i][0];
                mod.MaxAnchor = _ItemInventoryAnchors[i][1];
            }
        }

        private void UpdateCraft(ContainerItem item, int index)
        {
            
        }
        
        private void UpdateItem(ContainerItem item, int index)
        {
            Item aitem = item.Item;

            WObject itemRenderer = InventoryItemsRenders[index];
            Model mod = null;
            Label amount = null;
            if (itemRenderer == null)
            {
                
            }
            else
            {
                mod = itemRenderer.GetModule<Model>();
                amount = InventoryItemsAmount[index];
            }
            
            itemRenderer.LocalRotation = aitem.InventoryRotation;
            itemRenderer.LocalScale = aitem.InventoryScale;

            if (aitem.Identifier == "winecrash:air")
            {
                mod.Renderer.Mesh = null;
                amount.Text = "";
            }
            else
            {
                mod.Renderer.Mesh = aitem.Model;
                amount.Text = item.Amount > 1 ? item.Amount.ToString() : "";
            }

            
            
            
            amount.WObject.Rotation = Quaternion.Identity;
        }
        private void UpdateHotbarItem (ContainerItem item, int index)
        { 
            Item aitem = item.Item;

            WObject itemRenderer = HotbarItemsRenders[index];
            Model mod = null;
            Label amount = null;
            if (itemRenderer == null)
            {
                itemRenderer = HotbarItemsRenders[index] = CreateItemRenderer(Hotbar, out mod, out amount);
                itemRenderer.Name += " (Hotbar)";
                mod.Depth = 1000;
                amount.Depth = 1000;
                HotbarItemsAmount[index] = amount;
            }
            else
            {
                mod = itemRenderer.GetModule<Model>();
                amount = HotbarItemsAmount[index];
            }

            itemRenderer.LocalRotation = aitem.InventoryRotation;
            itemRenderer.LocalScale = aitem.InventoryScale;

            if (aitem.Identifier == "winecrash:air")
            {
                mod.Renderer.Mesh = null;
                amount.Text = "";
            }
            else
            {
                mod.Renderer.Mesh = aitem.Model;
                amount.Text = item.Amount > 1 ? item.Amount.ToString() : "";
            }

            
            mod.MinAnchor = _ItemHotbarAnchors[index][0];
            mod.MaxAnchor = _ItemHotbarAnchors[index][1];

            
            amount.WObject.Rotation = Quaternion.Identity;

            if(index == Player.LocalPlayer.HotbarSelectedIndex)
                UpdateHotbarSelected(item, index);
        }
        private static WObject CreateItemRenderer(WObject parent, out Model model, out Label amount)
        {
            WObject itemRenderer = new WObject("Item")
            {
                Parent = parent,
                Layer = (ulong) Layers.UI,
            };
            
            
            model = itemRenderer.AddModule<Model>();
            model.Renderer.Material = ItemCache.AtlasMaterial;
            model.KeepRatio = true;

            WObject labelRender = new WObject("Amount")
            {
                Parent = itemRenderer,
                Layer = (ulong)Layers.UI
            };
            amount = labelRender.AddModule<Label>();
            amount.Text = "";
            amount.AutoSize = true;
            amount.Aligns = TextAligns.Down | TextAligns.Right;
            amount.MinAnchor = new Vector2D(0.05,-0.3);
            amount.MaxAnchor = new Vector2D(1.0,0.3);

            return itemRenderer;
        }
        private void UpdateHotbarSelected(ContainerItem item, int index)
        {
            WObject wobj = PlayerEntity.PlayerHandWobject;

            Material material = ItemCache.AtlasMaterial;
            Mesh mesh = null;
            Item aitem;

            if (item == null)
            {
                aitem = ItemCache.Get<Item>("winecrash:air");
            }
            else
            {
                aitem = item.Item;
            }

            if (aitem.Identifier == "winecrash:air")
            {
                material = Player.LocalPlayer.Entity.SkinMaterial;
                mesh = PlayerEntity.PlayerRightArmMesh;
            }
            else
            {
                mesh = aitem.Model;
                ItemDisplayName(aitem);
            }

            wobj.LocalPosition = aitem.HandPosition;
            wobj.LocalRotation = aitem.HandRotation;
            wobj.LocalScale = aitem.HandScale;
            
            MeshRenderer mr = wobj.GetModule<MeshRenderer>();
            mr.Material = material;
            mr.Mesh = mesh;

            Image img = HotbarSelector.GetModule<Image>();

            Vector2D min = new Vector2D(-0.0055D, 0.01D);
            Vector2D max = new Vector2D(0.1265, 1.0725D);

            double magicNumber = 0.989D;
            min.X += (index / 9D) * magicNumber;
            max.X += (index / 9D) * magicNumber;

            img.MinAnchor = min;
            img.MaxAnchor = max;
        }

        private Random r = new Random();
        protected override void EarlyUpdate()
        {
            if (Player.LocalPlayer.ContainerOpened)
            {
                Vector2D scales = ItemSlotScale;
                Parallel.For(0, _colliders.Length, i =>
                {
                    if (_colliders[i]) _colliders[i].Extents = new Vector3D(scales, 10000);
                });
            }
        }

        protected override void Update()
        {
            if (Input.IsPressing(Keys.E))
            {
                if (Player.LocalPlayer.ContainerOpened)
                {
                    Player.LocalPlayer.CloseContainer();
                }
                else
                {
                    Player.LocalPlayer.OpenContainer(Player.LocalPlayer);
                }
            }

            if (Input.IsPressing(Keys.Escape))
            {
                Player.LocalPlayer.CloseContainer();
            }

            if (Input.IsPressed(Keys.R))
                Player.LocalPlayer.AddItemFast(new ContainerItem(ItemCache.GetIdentifier((ushort)r.Next((int)ItemCache.TotalItems))));

            AnimateDisplayName();

            Player.LocalPlayer.HotbarSelectedIndex -= Input.MouseScrollDeltaStep;
            
            int index = HoveringIndex();

            if(Player.LocalPlayer.ContainerOpened)
                if (index != -1 && Input.IsPressing(Keys.MouseLeftButton)) Player.LocalPlayer.MainGrab(index);
                else if (index != -1 && Input.IsPressing(Keys.MouseRightButton)) Player.LocalPlayer.AltGrab(index);
        }

        protected override void LateUpdate()
        {
            if (HoveredTooltipWObject.Enabled = Player.LocalPlayer.ContainerOpened)
            {
                int index = HoveringIndex();
                if (HoveredTooltipWObject.Enabled = index != -1)
                {
                    if (Player.LocalPlayer.Items[index] && 
                        Player.LocalPlayer.Items[index].Item != null && 
                        Player.LocalPlayer.Items[index].Item.Identifier != "winecrash:air")
                        HoveredTooltip.Label.Text = Game.Language.GetText(
                            Player.LocalPlayer.Items[index].Item.DisplayName);
                    else HoveredTooltipWObject.Enabled = false;
                }
                
                Vector2D rawmp = Input.MousePosition;
                Vector2D ss = Graphics.Window.SurfaceResolution;
                
                if(InventoryItemsRenders[45])
                    InventoryItemsRenders[45].GetModule<Model>().Shift = new Vector3D(rawmp, -1000);
            }
        }

        protected override void OnRender()
        {
            if (Player.LocalPlayer && Player.LocalPlayer.Entity)
            {
                Vector3D pos = Player.LocalPlayer.Entity.WObject.Position;
                DebugPosition.Text = $"XYZ: {pos.X:F3} / {pos.Y:F3} / {pos.Z:F3}";
            }

            HotbarImage.MinAnchor = new Vector2D(0.25D - UIScale, 0.0D);
            HotbarImage.MaxAnchor = new Vector2D(0.75D + UIScale, 1.0D);
            HotbarImage.Shift = Vector3D.Down * ((Graphics.Window.SurfaceResolution.Y / 2.0D) - HotbarImage.GlobalScale.Y / 2.0D);
        }

        protected override void OnEnable()
        {
            Crosshair.Enabled = true;
            Hotbar.Enabled = true;
            DebugPanel.Enabled = true;
        }

        protected override void OnDisable()
        {
            Crosshair.Enabled = false;
            Hotbar.Enabled = false;
            DebugPanel.Enabled = false;
        }

        protected override void OnDelete()
        {
            if (Instance == this) Instance = null;
            Crosshair?.Delete();
            Crosshair = null;

            HotbarImage = null;
            HotbarSelector?.Delete();
            HotbarSelector = null;

            HotbarImage = null;
            Hotbar?.Delete();
            Hotbar = null;
            
            Inventory?.Delete();
            Inventory = null;
            
            GlobalInventoryBackground?.Delete();
            GlobalInventoryBackground = null;

            ItemNameLabel.WObject.Delete();
            ItemNameLabel = null;
            
            HotbarItemsAmount = null;
            HotbarItemsRenders = null;
            
            HoveredTooltipWObject.Delete();
            HoveredTooltipWObject = null;
            HoveredTooltip = null;

            Player.LocalPlayer.OnHotbarUpdate -= UpdateHotbarItem;
            Player.LocalPlayer.OnHotbarSelectedChange -= UpdateHotbarSelected;
            Player.LocalPlayer.OnContainerToggle -= TogglePlayerInventory;
            Player.LocalPlayer.OnContainerClose -= PlayerCloseInventory;
            Player.LocalPlayer.OnContainerOpen -= PlayerOpenInventory;
            Player.LocalPlayer.OnItemUpdate -= UpdateItem;
        }
    }
}