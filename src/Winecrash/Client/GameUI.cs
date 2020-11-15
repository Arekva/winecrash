using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        private static readonly Vector2D[][] _ItemHotbarAnchors = new Vector2D[9][]
        {
            // 0
            new Vector2D[2] { new Vector2D(3/182.0D,19/22.0D), new Vector2D(19/182.0D,3/22.0D) },
            //1
            new Vector2D[2] { new Vector2D(23/182.0D,19/22.0D), new Vector2D(39/182.0D,3/22.0D) },
            //2
            new Vector2D[2] { new Vector2D(43/182.0D,19/22.0D), new Vector2D(59/182.0D,3/22.0D) },
            //3
            new Vector2D[2] { new Vector2D(63/182.0D,19/22.0D), new Vector2D(79/182.0D,3/22.0D) },
            //4
            new Vector2D[2] { new Vector2D(83/182.0D,19/22.0D), new Vector2D(99/182.0D,3/22.0D) },
            //5
            new Vector2D[2] { new Vector2D(103/182.0D,19/22.0D), new Vector2D(119/182.0D,3/22.0D) },
            //6
            new Vector2D[2] { new Vector2D(123/182.0D,19/22.0D), new Vector2D(139/182.0D,3/22.0D) },
            //7
            new Vector2D[2] { new Vector2D(143/182.0D,19/22.0D), new Vector2D(159/182.0D,3/22.0D) },
            //8
            new Vector2D[2] { new Vector2D(163/182.0D,19/22.0D), new Vector2D(179/182.0D,3/22.0D) },
        };

        private WObject[] HotbarItemsRenders { get; set; } = new WObject[9];
        private Label[] HotbarItemsAmount { get; set; } = new Label[9];
        
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
        }

        private void CreateInventoryFocus()
        {
            GlobalInventoryBackground = new WObject("Inventory Background")
            {
                Parent = Canvas.Main.WObject,
                Layer = (ulong) Layers.UI
            };

            Image img = GlobalInventoryBackground.AddModule<Image>();
            img.Picture = Texture.Blank;
            img.Color = new Color256(0.2,0.2,0.2,0.75);
            GlobalInventoryBackground.GetModule<ImageRenderer>().DrawOrder = 1000;
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
            Inventory.GetModule<ImageRenderer>().DrawOrder = 1000;

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
            lb.Renderer.DrawOrder = 1000;
            
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
        }
        
        private WObject itemPreview;
        protected override void Start()
        {

        }

        private static double ItemNameCooldown = 0.0D;
        public static double ItemNameTime = 2.0D;
        public static double ItemNameDisappearTime = 1.0D;
        

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
            ItemNameCooldown -= Time.DeltaTime;
            
            if(ItemNameCooldown < 0.0)
                ItemNameLabel.Enabled = false;
            else if (ItemNameCooldown < ItemNameTime)
            {
                Color256 col = ItemNameLabel.Color;
                col.A = WMath.Clamp(ItemNameCooldown / ItemNameDisappearTime,0,1);
                ItemNameLabel.Color = col;
            }
        }
        
        //private void UpdateInventoryItems(ContainerItem)

        private void UpdateHotbarItem(ContainerItem item, int index)
        { 
            Item aitem = item.Item;

            WObject itemRenderer = HotbarItemsRenders[index];
            Model mod = null;
            Label amount = null;
            if (itemRenderer == null)
            {
                itemRenderer = HotbarItemsRenders[index] = new WObject("Item")
                {
                    Parent = Hotbar,
                    Layer = (ulong) Layers.UI,
                };
                
                mod = itemRenderer.AddModule<Model>();
                mod.Renderer.Material = ItemCache.AtlasMaterial;
                mod.KeepRatio = true;
                
                WObject labelRender = new WObject("Amount")
                {
                    Parent = itemRenderer,
                    Layer = (ulong)Layers.UI
                };
                amount = HotbarItemsAmount[index] = labelRender.AddModule<Label>();
                amount.Text = "";
                amount.AutoSize = true;
                amount.Aligns = TextAligns.Down | TextAligns.Right;
                amount.MinAnchor = new Vector2D(0.05,-0.3);
                amount.MaxAnchor = new Vector2D(1.0,0.3);
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
            if(Input.IsPressing(Keys.R))
                Player.LocalPlayer.AddItemFast(new ContainerItem(ItemCache.GetIdentifier((ushort)r.Next((int)ItemCache.TotalItems))));

            AnimateDisplayName();

            Player.LocalPlayer.HotbarSelectedIndex -= Input.MouseScrollDeltaStep;
        }

        protected override void LateUpdate()
        {
            Vector3D pos = Player.LocalPlayer.Entity.WObject.Position;
            DebugPosition.Text = $"XYZ: {pos.X:#.###} / {pos.Y:#.###} / {pos.Z:#.###}";
        }

        protected override void OnRender()
        {
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

            Player.LocalPlayer.OnHotbarUpdate -= UpdateHotbarItem;
            Player.LocalPlayer.OnHotbarSelectedChange -= UpdateHotbarSelected;
            Player.LocalPlayer.OnContainerToggle -= TogglePlayerInventory;
            Player.LocalPlayer.OnContainerClose -= PlayerCloseInventory;
            Player.LocalPlayer.OnContainerOpen -= PlayerOpenInventory;
        }
    }
}